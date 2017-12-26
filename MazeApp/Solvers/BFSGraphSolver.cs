using System;
using System.Collections.Generic;
using System.Linq;
using GraphsLibrary;
using MazeApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace MazeApp.Solvers
{
    public class BFSGraphSolver : IMazeSolver
    {

        private readonly ILogger _logger;

        public BFSGraphSolver(ILogger<BFSGraphSolver> logger)
        {
            _logger = logger;
        }

               
        public void Solve(IMazeGrid maze, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {
            // the maze graph
            Graph<IMazeCell> graph = maze.GenerateGraph();


            // List of Accepted solutions used to reset the visited nodes
            List<List<GraphNode<IMazeCell>>> solutions = new List<List<GraphNode<IMazeCell>>>(); 

            // Que of graph nodes
            Queue<GraphNode<IMazeCell>> queue = new Queue<GraphNode<IMazeCell>>();

            // Directory of Predecessor
            Dictionary<GraphNode<IMazeCell>, GraphNode<IMazeCell>> predecessors = new Dictionary<GraphNode<IMazeCell>, GraphNode<IMazeCell>>();
                        
            // List visited nodes
            List<GraphNode<IMazeCell>> visited = new List<GraphNode<IMazeCell>>();

            GraphNode<IMazeCell> currentNode = null;

            GraphNode<IMazeCell> startNode = (GraphNode<IMazeCell>)graph.Nodes.FindByValue(maze.Start);

            queue.Enqueue(startNode);

            
            
            while (queue.Count > 0)
            {                
                currentNode = queue.Dequeue();

                _logger.LogTrace($"Dequeue Node({currentNode.Value.ToString()})");

                if (visited.Contains(currentNode))
                    continue;

                visited.Add(currentNode);

                if (maze.Finish.Equals(currentNode.Value))
                {

                    _logger.LogTrace($"Finish Node({currentNode.Value.ToString()})");

                    IEnumerable<GraphNode<IMazeCell>> solvedPath = TraceSolvedPath(currentNode, predecessors);

                    // callback Action and returns path.
                    List<IMazeCell> cbPath = new List<IMazeCell>();

                    foreach (var node in solvedPath)
                    {
                        cbPath.Add(node.Value);
                    }

                    solvedResultCallback(cbPath);
                                        
                    break;
                }

                foreach (GraphNode<IMazeCell> neighbor in currentNode.Neighbors)
                {

                    if (!visited.Contains(neighbor) && !queue.Contains(neighbor))
                    {
                        // enquew
                        queue.Enqueue(neighbor);

                        // keep track of Predecessor
                        predecessors[neighbor] = currentNode;

                        _logger.LogTrace($"Enqueue Node: {neighbor.Value.ToString()} Predecessor: {currentNode.Value.ToString()}");
                                                
                        
                    }
                    else
                        _logger.LogTrace($"Node({neighbor.Value.ToString()}) is visited");
                    

                }

            }
            
        }

       

        /// <summary>
        /// Traces the solved path and builds the internal BFS tree.
        /// </summary>
        private IEnumerable<GraphNode<IMazeCell>> TraceSolvedPath(GraphNode<IMazeCell> currentNode, Dictionary<GraphNode<IMazeCell>, GraphNode<IMazeCell>> predecessors)
        {
            List<GraphNode<IMazeCell>> pathTrace = new List<GraphNode<IMazeCell>>();

            while (currentNode != null)
            {
                if( !pathTrace.Contains(currentNode))
                    pathTrace.Add(currentNode);

                if (predecessors.ContainsKey(currentNode))
                    currentNode = predecessors[currentNode];
                else
                    currentNode = null;
            }

            pathTrace.Reverse();

            return pathTrace;
        }

    }
}
