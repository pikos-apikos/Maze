using GraphsLibrary;
using MazeApp.Maze;
using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp.Interfaces
{
    public interface IMazeGrid
    {
        int Height { get; set; }
        int Width { get; set; }

        IMazeCell[,] MazeMap { get; set; }

        // Define the start point 
        IMazeCell Start { get; set; } 

        // Define the finish point
        IMazeCell Finish { get; set; }

        // Load data from some source
        void Load(IMazeLoader loader);

        // Used by the client to solve the maze.
        void Solve(IMazeSolver solver, Action<IEnumerable<IMazeCell>> solvedResultCallback);

        
        // Lists all neighbor cells for a given node.
        IEnumerable<IMazeCell> GetNeighborCells(IMazeCell curCell);

        // Gets a specific cells given the position.
        IMazeCell GetNode(int row, int col);

        // Gets all cells for the maze.
        IEnumerator<IMazeCell> GetNodes();

        // Get a graph representation of the grid 
        Graph<IMazeCell> GenerateGraph();

    }
}
