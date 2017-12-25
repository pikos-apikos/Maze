using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp.Exceptions
{
    class InvalidFileSizeException: Exception
    {
        public InvalidFileSizeException() : base()
        {

        }
        /// <summary>
        /// Invalid File Size Exception
        /// </summary>
        /// <param name="size">File Size</param>
        /// <param name="uxPhrase">Useful message</param>
        public InvalidFileSizeException(long size, string uxPhrase)
            : base(String.Format("File size {0}kb is either too big or too small, {1}", Math.Abs(size /= 1024).ToString(), uxPhrase))
        {

        }
    }
}
