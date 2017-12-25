using GraphsLibrary;
using MazeApp.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeApp.Solvers
{
    public class DijkstraGraphSolver : IMazeSolver
    {
        private readonly ILogger _logger;

        public DijkstraGraphSolver(ILogger<DijkstraGraphSolver> logger)
        {

            _logger = logger;

        }

        public void Dijkstra(GraphNode<IMazeCell> currentNode, GraphNode<IMazeCell> endNode, LinkedList<GraphNode<IMazeCell>> visited, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {

        }

        public void Solve(IMazeGrid maze, Action<IEnumerable<IMazeCell>> solvedResultCallback)
        {

            Graph<IMazeCell> graph = maze.GenerateGraph();
            
            GraphNode<IMazeCell> startNode = (GraphNode<IMazeCell>)graph.Nodes.FindByValue(maze.Start);

            
            Dictionary<GraphNode<IMazeCell>, GraphNode<IMazeCell>> predecessors = new Dictionary<GraphNode<IMazeCell>, GraphNode<IMazeCell>>();
            List<GraphNode<IMazeCell>> nodes = new List<GraphNode<IMazeCell>>();
            Dictionary<GraphNode<IMazeCell>, int> distances = new Dictionary<GraphNode<IMazeCell>, int>();

            GraphNode<IMazeCell> currentNode = null;

            // read all nodes and init distances
            foreach (GraphNode<IMazeCell> vertex in graph.Nodes)
            {
                // start node has 0 distance, all over nodes are unknown and set to Max
                if (vertex.Value.Equals(maze.Start))                
                    distances[vertex] = 0;
                else
                    distances[vertex] = int.MaxValue;

                nodes.Add(vertex);
            }

            while (nodes.Count != 0)
            {
                // Find current shortest path point to explore
                nodes.Sort((x, y) => distances[x] - distances[y]);

                currentNode = nodes[0];

                nodes.Remove(currentNode);

                if (currentNode.Value.Equals(maze.Finish))
                {
                    
                    IEnumerable<IMazeCell> solvedPath = TraceSolvedPath(currentNode, predecessors).Reverse();
                                        
                    solvedResultCallback(solvedPath); // Calls the callback Action and returns the path.

                    break;
                }

                if (distances[currentNode] == int.MaxValue)
                {
                    break;
                }

                
                foreach (GraphNode<IMazeCell> neighbor in currentNode.Neighbors)
                {
                    var indx = currentNode.Neighbors.IndexOf(neighbor);

                    // distance of current node + Neighbors Cost ( Cost = paths between nodes )
                    int newDistance = distances[currentNode] + currentNode.Costs[indx];
                    
                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        predecessors[neighbor] = currentNode;
                    }
                }
            }

        }

        /// <summary>
        /// Traces the solved path
        /// </summary>
        private IEnumerable<IMazeCell> TraceSolvedPath(GraphNode<IMazeCell> currentNode, Dictionary<GraphNode<IMazeCell>, GraphNode<IMazeCell>> predecessors)
        {
            List<IMazeCell> pathTrace = new List<IMazeCell>();

            while (currentNode != null)
            {
                pathTrace.Add(currentNode.Value);

                if (predecessors.ContainsKey(currentNode))
                    currentNode = predecessors[currentNode];
                else
                    currentNode = null;
            }

            return pathTrace;
        }

    }
}
