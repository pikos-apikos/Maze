using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp
{
    /// <summary>
    /// Configuration parameters of the maze app 
    /// </summary>
    public class MazeSettings
    {

        /// <summary>
        /// Path of the maze file
        /// </summary>
        public string MazeFile { get; set; }

        /// <summary>
        /// Solving algorithm
        /// </summary>
        public string MazeSolver { get; set; }

        /// <summary>
        /// Expected minimum Grid width
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// Expected minimum Grid height
        /// </summary>
        public int MinHeight { get; set; }

        /// <summary>
        /// Expected emty Cell character
        /// </summary>
        public string OpenChar { get; set; }

        /// <summary>
        /// Expected wall Cell character
        /// </summary>
        public string WallChar { get; set; }

        /// <summary>
        /// Expected start Cell character 
        /// </summary>
        public string StartChar { get; set; }

        /// <summary>
        /// Expected end Cell character ()
        /// </summary>
        public string FinishChar { get; set; }

    }
}
