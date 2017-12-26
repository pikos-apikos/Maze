using MazeApp.Interfaces;
using MazeApp.Loaders;
using MazeApp.Maze;
using MazeApp.Solvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MazeApp.Tests
{
    public class BFSRecursiveGraphSolverShould
    {

        IServiceProvider servicesProvider;
        ILogger<MazeTextLoader> logger;
        BFSRecursiveGraphSolver sut;

        public BFSRecursiveGraphSolverShould()
        {

            // Load DI
            servicesProvider = MazeApp.Program.BuildDI(null, null);
            logger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeTextLoader>();

            var slogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<BFSRecursiveGraphSolver>();

            // System Under Test  
            sut = new BFSRecursiveGraphSolver(slogger);

        }

        /// <summary>
        /// Using the default configuration. BFS should return one or more solutions
        /// </summary>
        [Fact]
        public void SolveValidMaze()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

            var glogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            IMazeGrid grid = new MazeGrid(glogger);

            grid.Load(loader);

            // Keep a list of solutions
            var solutions = new List<IEnumerable<IMazeCell>>();

            grid.Solve(sut,
                (solvedPath) =>
                {
                    if (solvedPath.Count() > 0)
                    {
                        solutions.Add(solvedPath);
                    }

                });

            // One ore more solution
            Assert.True(solutions.Count() >= 1);

            foreach (var solvedPath in solutions)
            {
                // First item = Start
                Assert.True(solvedPath.First().Equals(grid.Start));

                // Last Item = Finish
                Assert.True(solvedPath.Last().Equals(grid.Finish));

            }
            
        }


        /// <summary>
        /// If the maze has no solution solver must return 0 solutions
        /// </summary>
        [Fact]
        public void NotSolveInvalidMaze()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();
            settings.MazeFile = ".\\TestFiles\\maze8x6-error-unsolvable.txt";

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

            var glogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            IMazeGrid grid = new MazeGrid(glogger);

            grid.Load(loader);


            var solutions = new List<IEnumerable<IMazeCell>>();

            grid.Solve(sut,
                (solvedPath) =>
                {
                    if (solvedPath.Count() > 0)
                    {
                        solutions.Add(solvedPath);
                    }

                });

            // Empty solution
            Assert.Empty(solutions);

        }

    }
}
