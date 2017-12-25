using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp.Interfaces
{
    /// <summary>
    /// Represents an interface for solver.
    /// Solvers accept a maze and report through a callback function
    /// </summary>
    public interface IMazeSolver
    {
        void Solve(IMazeGrid maze, Action<IEnumerable<IMazeCell>> solvedResultCallback);
    }
}
