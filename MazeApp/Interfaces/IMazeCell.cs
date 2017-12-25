using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp.Interfaces
{

    /// <summary>
    /// Valid states for a maze cell.
    /// </summary>
    public enum CellState : int
    {
        Path = 0,
        Wall = 1
    }

    public interface IMazeCell
    {
        int Row { get; }    //'y' coordinate
        int Col { get; }    //'x' coordinate
         
        CellState State { get; set; } // State information 

        IMazeCell[] Neighbours { get; set; }

    }
}
