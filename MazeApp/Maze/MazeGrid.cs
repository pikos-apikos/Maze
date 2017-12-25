using GraphsLibrary;
using MazeApp.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;



namespace MazeApp.Maze
{
    public class MazeGrid : IMazeGrid
    {

        private MazeCell _start = null;
        private MazeCell _finish = null;

        private IMazeCell[,] _mazeMap = null;
        
        private int _width = 0;
        private int _height = 0;

        
        private readonly ILogger _logger;

        //Indicates the total width of the maze. Used by the solver during its initialization.
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        //Indicates the total height of the maze. Used by the solver during its initialization.
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        //Indicates the start point of the maze
        public IMazeCell Start
        {
            get { return _start; }
            set { _start = (MazeCell)value; }
        }

        //Indicate the end point of the maze
        public IMazeCell Finish
        {
            get { return _finish; }
            set { _finish = (MazeCell)value; }
        }

        
       
        public IMazeCell[,] MazeMap
        {
            get { return _mazeMap; }
            set { _mazeMap = value; }
        }

               
        public MazeGrid(ILogger<MazeGrid> logger)
        {                   
            _logger = logger;            
        }

        /// <summary>
        /// Load data from an external source
        /// </summary>
        /// <param name="loader"></param>
        public void Load(IMazeLoader loader)
        {

            if (loader == null)
                throw new ArgumentNullException("Loader cannot be null", "loader");
                      
            
            _logger.LogInformation($"Loading Data Using: {loader.GetType().Name}");

            loader.Load(this);

        }


        /// <summary>
        /// Function that solves the maze using the solver.
        /// </summary>
        public void Solve(IMazeSolver solver, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {
            if (solver == null)
                throw new ArgumentNullException("Solver cannot be null", "solver");

            if (solvedResultCallback == null)
                throw new ArgumentNullException("Please provide a callback action", "solvedResultCallback");


            _logger.LogInformation($"Solve Using: {solver.GetType().Name}");


            //calls solver's solve method.
            solver.Solve(this, (solvedPath) =>
            {
                if (solvedPath == null)
                    solvedResultCallback(new List<IMazeCell>(0));//return a empty path if the solver could not solve the maze.
                else
                    solvedResultCallback(solvedPath);
            });
        }


        /// <summary>
        /// Generates a list of cell connected in a graph, that reduses the steps we need to take in order to solve the maze
        /// the missing Path cells are represented as distance between nodes and the initial MazeCell are updated with orientation parametes
        /// </summary>
        public Graph<IMazeCell> GenerateGraph()
        {
            Graph<IMazeCell> _graph = new Graph<IMazeCell>();

            // keeps track of the last left node that creates a horizontaly path
            GraphNode<IMazeCell> leftnode = null;

            // keeps track of the nodes that create a vertical path
            GraphNode<IMazeCell>[] topnodes = new GraphNode<IMazeCell>[_width];


            // Scan Grid from left to right
            for (int y = 0; y < _height; y++)
            {

                // Initialize previous, current and next states
                CellState prev = CellState.Wall;
                CellState cur = CellState.Wall;
                CellState next = GetNode(y, 0).State;

                for (int x = 0; x < _width; x++)
                {

                    int nextX = (x + 1);

                    prev = cur;
                    cur = next;

                    if (_width == nextX) // we 've reached the border
                        next = CellState.Wall;
                    else
                        next = GetNode(y, nextX).State;


                    GraphNode<IMazeCell> node = null;

                    if (cur == CellState.Wall)
                        continue;

                    var curNode = GetNode(y, x);

                    if (prev == CellState.Path)
                    {
                        if (next == CellState.Path)
                        {
                            // PATH PATH PATH
                            // Create node only if paths above or below.                            
                            if ((_start.Equals(curNode)) || (_finish.Equals(curNode)) || (y > 0 && GetNode(y - 1, x).State == CellState.Path) || (y < _height - 1 && GetNode(y + 1, x).State == CellState.Path))
                            {

                                leftnode.Value.Neighbours[1] = curNode;
                                curNode.Neighbours[3] = leftnode.Value;

                                node = new GraphNode<IMazeCell>
                                {
                                    Value = curNode
                                };

                                _graph.AddNode(node);

                                int cost = node.Value.Col - leftnode.Value.Col;

                                _graph.AddUndirectedEdge(node, leftnode, cost);

                                _logger.LogTrace($"Connected: {leftnode.Value.ToString()} :W {cost} E: {node.Value.ToString()}");

                                // new leftnode is node 
                                leftnode = node;

                            }


                        }
                        else
                        {
                            // PATH PATH WALL
                            // Create path at end of corridor                            
                            leftnode.Value.Neighbours[1] = curNode;
                            curNode.Neighbours[3] = leftnode.Value;

                            node = new GraphNode<IMazeCell>
                            {
                                Value = curNode,
                            };

                            _graph.AddNode(node);

                            int cost = node.Value.Col - leftnode.Value.Col;

                            _graph.AddUndirectedEdge(node, leftnode, cost);

                            _logger.LogTrace($"Connected: {leftnode.Value.ToString()} :W {cost} E: {node.Value.ToString()}");

                            // no way to connect, reset 
                            leftnode = null;

                        }
                    }
                    else
                    {

                        if (next == CellState.Path)
                        {

                            // WALL PATH PATH
                            // Create path at start of corridor
                            node = new GraphNode<IMazeCell>
                            {
                                Value = curNode
                            };

                            leftnode = node;

                            _graph.AddNode(node);

                            _logger.LogTrace($"New leftnode: {node.Value.ToString()}");

                        }
                        else
                        {
                            //      WALL
                            // WALL PATH WALL
                            //      WALL
                            // Create node only if in dead end??
                            if ((y - 1 < 0) || (GetNode(y - 1, x).State == CellState.Wall) || (y + 1 > _height) || (GetNode(y + 1, x).State == CellState.Wall))
                            {

                                _logger.LogTrace("dead end ");

                            }

                        }

                    }

                    // we can assume we can connect N-S somewhere
                    if (node != null)
                    {


                        if (y > 0 && GetNode(y - 1, x).State == CellState.Path)
                        {
                            //There is a path above , connect to the latest top node 
                            var t = topnodes[x];
                            if (t != null)
                            {
                                t.Value.Neighbours[2] = node.Value;
                                node.Value.Neighbours[0] = t.Value;

                                int cost = node.Value.Row - t.Value.Row;

                                _graph.AddUndirectedEdge(node, t, cost);

                                _logger.LogTrace($"Connected: {t.Value.ToString()} :N {cost} S: {node.Value.ToString()}");
                            }

                        }

                        // create the new top node if there is a path below 
                        if ((y < _height - 1 && GetNode(y + 1, x).State == CellState.Path))
                            topnodes[x] = node;
                        else
                            topnodes[x] = null;

                    }

                }

            }

            return _graph;
        }



        /// <summary>
        /// Gets adjacents for a node. Any node can have at most 8 adjacents.
        /// </summary>
        public IEnumerable<IMazeCell> GetNeighborCells(IMazeCell curNode)
        {
            int rowPosition = curNode.Row;
            int colPosition = curNode.Col;

            //Validate given node bounds 
            if (rowPosition < 0 || rowPosition >= _height || colPosition < 0 || colPosition >= _width) 
                throw new IndexOutOfRangeException();

            List<IMazeCell> neighbors = new List<IMazeCell>(4);

            // move above an below 
            for (int i = rowPosition - 1; i <= rowPosition + 1; i++)
            {
                if (i < 0 || i >= _height || i == rowPosition) //eliminates out of bounds from being sent as adjacents.
                    continue;
                neighbors.Add(GetNode(i, colPosition));
            }

            // move light and left
            for (int j = colPosition - 1; j <= colPosition + 1; j++)
            {
                if ( j < 0 || j >= _width || j == colPosition) //eliminates out of bounds from being sent as adjacents.
                    continue;
                neighbors.Add(GetNode(rowPosition, j));
            }

            return neighbors;
        }

        /// <summary>
        /// Gets a node.
        /// </summary>
        public IMazeCell GetNode(int row, int col)
        {
            GuardPosition(row, col, "row\\col");
            return _mazeMap[row, col];
        }

        /// <summary>
        /// Gets all maze nodes.
        /// </summary>
        public IEnumerator<IMazeCell> GetNodes()
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    yield return _mazeMap[i, j];
        }

        
        # region GuardMethods
        /// <summary>
        /// Validates node bounds
        /// </summary>
        /// <param name="rowPosition"></param>
        /// <param name="colPosition"></param>
        /// <param name="exceptionParamName"></param>
        private void GuardPosition(int rowPosition, int colPosition, string exceptionParamName)
        {
            if (rowPosition < 0 || rowPosition >= _height || colPosition < 0 || colPosition >= _width)
                throw new ArgumentException("The supplied node is out of bounds", exceptionParamName);
        }
        #endregion


    }
}
