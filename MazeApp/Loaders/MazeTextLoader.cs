using MazeApp.Exceptions;
using MazeApp.Interfaces;
using MazeApp.Maze;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MazeApp.Loaders
{
    public class MazeTextLoader : IMazeLoader
    {
        private readonly MazeSettings _mazeSettings;        
        private readonly ILogger _logger;

        public MazeTextLoader(MazeSettings mazeSettings, ILogger<MazeTextLoader> logger)
        {
            _mazeSettings = mazeSettings;
            _logger = logger;
            
        }

       
        public void Load(IMazeGrid grid)
        {

            if (grid == null)
                throw new ArgumentNullException("Maze Grid cannot be null", "grid");

            // Read file into a list of strings and validate lines
            var lines = new List<string>();

            int mazeWidth = 0;
            int mazeHeight = 0;
            try
            {

                
                ValidateMazeFile();


                _logger.LogInformation($"Reading Maze File: {_mazeSettings.MazeFile}");

                using (StreamReader sr = new StreamReader(_mazeSettings.MazeFile))
                {

                    while (sr.Peek() >= 0)
                    {

                        // Read the stream to a string, trim to remove white space.
                        string line = sr.ReadLine();

                        // validate line 
                        if (string.IsNullOrWhiteSpace(line))
                            throw new InvalidDataException("File can not contain white space"); // can i kick it ? 


                        // Initialize maze width or validate width
                        if (mazeHeight == 0)
                        {
                            mazeWidth = line.Length;
                        }
                        else
                        {
                            if (!mazeWidth.Equals(line.Length))
                                throw new InvalidDataException("Lines must have the same width");
                        }


                        lines.Add(line);
                        mazeHeight++;

                    }

                }

            }
            catch (Exception e) 
            {

                // Invalid file or file access 

                _logger.LogError(e.Message);
                throw new Exception("Invalid file or file access ", e);
                
            }

            if (lines.Count == 0)
            {
                throw new Exception("File is empty or does not contain valid lines");
            }


            LoadMazeMapFromStringList(grid, lines, mazeHeight, mazeWidth); 
            

        }

        
        /// <summary>
        /// Construct the maze grid and validate for characters and Start/End Point 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="lines"></param>
        /// <param name="mazeHeight"></param>
        /// <param name="mazeWidth"></param>
        private void LoadMazeMapFromStringList(IMazeGrid grid, List<string> lines, int mazeHeight, int mazeWidth)
        {

            _logger.LogInformation($"I found a {mazeWidth} x {mazeHeight} Maze. Moving on to Maze Construction & Validation");

            // Initialize 2D array
            grid.MazeMap = new IMazeCell[mazeHeight, mazeWidth];
            grid.Height = mazeHeight;
            grid.Width = mazeWidth;


            int i = 0;
            lines.ForEach(line =>
            {

                for (int j = 0; j < mazeWidth; j++)
                {

                    char c = line[j];

                    if (c == _mazeSettings.WallChar[0])
                    {
                        grid.MazeMap[i, j] = new MazeCell(i, j) { State = CellState.Wall };

                        continue;
                    }
                    else if (c == _mazeSettings.StartChar[0])
                    {
                        grid.MazeMap[i, j] = new MazeCell(i, j) { State = CellState.Path };

                        if (grid.Start == null)
                            grid.Start = grid.MazeMap[i, j];
                        else
                            throw new Exception("I do not support more than one starting points");
                    }
                    else if (c == _mazeSettings.FinishChar[0])
                    {
                        grid.MazeMap[i, j] = new MazeCell(i, j) { State = CellState.Path };

                        if (grid.Finish == null)
                            grid.Finish = grid.MazeMap[i, j];
                        else
                            throw new Exception("I do not support more than one ending points");
                    }
                    else if (c == _mazeSettings.OpenChar[0])
                    {
                        grid.MazeMap[i, j] = new MazeCell(i, j) { State = CellState.Path };
                    }
                    else
                    {
                        //Invalid Char.                        
                        throw new Exception(string.Format("I found a char {0} that this maze was not configured to handle.", c.ToString()));
                    }
                }

                i++;
            });

            // Finaly validate that there is a start and an end point 
            if (grid.Start == null || grid.Finish == null)
            {
                throw new Exception($"This maze does not have {((grid.Start == null) ? "a start" : "an end")} point");
            }

        }

        /// <summary>
        /// Text loader validation. Validate file 
        /// check extention and size before loading into a stream
        /// </summary>
        private void ValidateMazeFile()
        {

            _logger.LogTrace($"Validating Maze File: {_mazeSettings.MazeFile}");

            // extention 
            string ext = Path.GetExtension(_mazeSettings.MazeFile);

            if (ext != ".txt")
            {
                throw new InvalidFileTypeException(ext, "I only support .txt files");
            }

            // calculate the approximate expected file size 
            long maxBytes = 0;
            long tmp = Encoding.UTF8.GetByteCount(_mazeSettings.StartChar);

            if (maxBytes < tmp)
                maxBytes = tmp;

            tmp = Encoding.UTF8.GetByteCount(_mazeSettings.FinishChar);
            if (maxBytes < tmp)
                maxBytes = tmp;

            tmp = Encoding.UTF8.GetByteCount(_mazeSettings.OpenChar);
            if (maxBytes < tmp)
                maxBytes = tmp;

            tmp = Encoding.UTF8.GetByteCount(_mazeSettings.WallChar);
            if (maxBytes < tmp)
                maxBytes = tmp;

            long maxSize = (long)int.MaxValue * 2;

            long maxExpextedFileSize = maxSize * maxBytes;

            long length = new System.IO.FileInfo(_mazeSettings.MazeFile).Length;

            if (length > maxExpextedFileSize || length == 0)
            {
                throw new InvalidFileSizeException(length, "please check the file and try again.");
            }

        }

    }

}
