using System;
using System.Collections.Generic;
using System.Text;

namespace MazeApp.Exceptions
{
    public class InvalidFileTypeException: Exception
    {
        public InvalidFileTypeException() : base()
        {

        }

        /// <summary>
        /// Invalid File Type Exception
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <param name="uxPhrase">Useful message</param>
        public InvalidFileTypeException(string extension, string uxPhrase)
            : base(String.Format("File type .{0} is invalid, {1}", extension, uxPhrase))
        {

        }

    }

}
