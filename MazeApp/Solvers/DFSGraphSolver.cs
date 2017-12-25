using System;
using System.Collections.Generic;
using System.Linq;
using GraphsLibrary;
using MazeApp.Interfaces;
using Microsoft.Extensions.Logging;

namespace MazeApp.Solvers
{
    /// <summary>
    /// Standard Depth-first search using Stack
    /// </summary>
    public class DFSGraphSolver : IMazeSolver
    {

        private readonly ILogger _logger;

        public DFSGraphSolver(ILogger<DFSGraphSolver> logger)
        {
            _logger = logger;
        }
        
        public void Solve(IMazeGrid maze, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {

            Graph<IMazeCell> graph = maze.GenerateGraph();

            // stack of graph nodes
            Stack<GraphNode<IMazeCell>> stack = new Stack<GraphNode<IMazeCell>>();

            // Routes Collection
            LinkedList<GraphNode<IMazeCell>> visited = new LinkedList<GraphNode<IMazeCell>>();  

            GraphNode<IMazeCell> startNode = (GraphNode<IMazeCell>)graph.Nodes.FindByValue(maze.Start);

            stack.Push(startNode);
                        
            GraphNode<IMazeCell> currentNode = null;

            while (stack.Count > 0)
            {

                currentNode = stack.Pop();

                _logger.LogTrace($"Poped Node: {currentNode.Value.ToString()}");

                if (visited.Contains(currentNode))
                    continue;

                visited.AddFirst(currentNode);

                if (maze.Finish.Equals(currentNode.Value))
                {
                                        
                    _logger.LogTrace($"Found Finish Node: {currentNode.Value.ToString()}");
                    
                    solvedResultCallback(TraceSolvedPath(visited));

                }

                // Examine neighbor nodes
                foreach (GraphNode<IMazeCell> neighbor in currentNode.Neighbors)
                {

                    if (!visited.Contains(neighbor))
                    {
                        
                        stack.Push(neighbor);

                        _logger.LogTrace($"Push Node: {neighbor.Value.ToString()}");
                                                
                    }
                    else
                        _logger.LogTrace($"Node: {neighbor.Value.ToString()} is visited");
                    
                }

            }

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
