using System;

namespace MazeApp.Interfaces
{
    /// <summary>
    /// Represents an interface for maze loaders.
    /// A Maze can be loaded from text, image, memory etc.. 
    /// </summary>
    public interface IMazeLoader
    {
        void Load(IMazeGrid grid);
    }
}
