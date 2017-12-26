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
    public class DFSGraphSolverShould
    {

        IServiceProvider servicesProvider;
        ILogger<MazeTextLoader> logger;
        DFSGraphSolver sut;

        public DFSGraphSolverShould()
        {

            // Load DI
            servicesProvider = MazeApp.Program.BuildDI(null, null);
            logger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeTextLoader>();

            var slogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DFSGraphSolver>();

            // System Under Test  
            sut = new DFSGraphSolver(slogger);

        }

        /// <summary>
        /// Using the default configuration. DFS Solver should return one solution
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

            // Single solution
            Assert.Single(solutions);

            // First Item = Start
            Assert.True(solutions.FirstOrDefault().First().Equals(grid.Start));

            // Last Item = Finish
            Assert.True(solutions.FirstOrDefault().Last().Equals(grid.Finish));

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
