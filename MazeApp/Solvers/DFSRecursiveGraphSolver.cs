using System;
using System.Collections.Generic;
using System.Linq;
using GraphsLibrary;
using MazeApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace MazeApp.Solvers
{
    /// <summary>
    /// Recursive Depth-first search using LinkedList instead of Stack
    /// </summary>
    public class DFSRecursiveGraphSolver: IMazeSolver
    {

        private readonly ILogger _logger;

        public DFSRecursiveGraphSolver(ILogger<DFSRecursiveGraphSolver> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Recursion Depth First 
        /// </summary>
        /// <param name="maze"></param>
        /// <param name="currentNode"></param>
        /// <param name="visited"></param>
        /// <param name="solvedResultCallback"></param>
        public void DepthFirst(GraphNode<IMazeCell> currentNode, GraphNode<IMazeCell> endNode, LinkedList<GraphNode<IMazeCell>> visited, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {
            _logger.LogTrace($"Poped Node: {currentNode.Value.ToString()}");

            // Examine neighbor nodes
            foreach (GraphNode<IMazeCell> neighbor in currentNode.Neighbors)
            {
                if (visited.Contains(neighbor))
                    continue;
               
                if (endNode.Value.Equals(neighbor.Value))
                {
                    _logger.LogTrace($"Found Finish Node: {neighbor.Value.ToString()}");

                    visited.AddFirst(neighbor);

                    _logger.LogTrace($"Push Node: {neighbor.Value.ToString()}");

                    solvedResultCallback(TraceSolvedPath(visited));

                    visited.RemoveFirst();

                    break;
                }
            }

            // recursion needs to come after visiting neighbors nodes
            foreach (GraphNode<IMazeCell> neighbor in currentNode.Neighbors)
            {
                if (visited.Contains(neighbor) || endNode.Value.Equals(neighbor.Value))
                    continue;

                visited.AddFirst(neighbor);
                _logger.LogTrace($"Push Node: {neighbor.Value.ToString()}");

                // Recursion
                DepthFirst(neighbor, endNode, visited, solvedResultCallback);

                visited.RemoveFirst();
            }
        }

        public void Solve(IMazeGrid maze, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {

            Graph<IMazeCell> graph = maze.GenerateGraph();

            // stack of graph nodes
            LinkedList<GraphNode<IMazeCell>> visited = new LinkedList<GraphNode<IMazeCell>>();  //collecting all routes

            GraphNode<IMazeCell> startNode = (GraphNode<IMazeCell>)graph.Nodes.FindByValue(maze.Start);
            GraphNode<IMazeCell> endNode = (GraphNode<IMazeCell>)graph.Nodes.FindByValue(maze.Finish);

            visited.AddFirst(startNode);

            // make the first call 
            DepthFirst(startNode, endNode, visited, solvedResultCallback);

        }

        /// <summary>
        /// Traces the solved path
        /// </summary>
        private IEnumerable<IMazeCell> TraceSolvedPath(LinkedList<GraphNode<IMazeCell>> visited)
        {
            // callback Action and returns path.
            List<IMazeCell> pathTrace = new List<IMazeCell>();

            foreach (var p in visited)
            {
                pathTrace.Add(p.Value);
            }

            pathTrace.Reverse();

            return pathTrace;
        }

    }
}
