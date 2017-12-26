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
using Moq;
using GraphsLibrary;

namespace MazeApp.Tests
{
    public class MazeGridShould
    {

        IServiceProvider servicesProvider;
        ILogger<MazeTextLoader> logger;

        MazeGrid sut;

        public MazeGridShould()
        {
            // Load DI
            servicesProvider = MazeApp.Program.BuildDI(null, null);
            logger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeTextLoader>();

            var mlogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            // System Under Test  
            sut = new MazeGrid(mlogger);

            // sut.Load

        }


        /// <summary>
        /// Using the default configuration. MazeGrid should be able to generate the grid without exceptions using a given loader
        /// </summary>
        [Fact]
        public void LoadFromGivenLoader()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

                                   
            sut.Load(loader);


            Assert.InRange<int>(sut.Width, settings.MinWidth, int.MaxValue);
            Assert.InRange<int>(sut.Height, settings.MinHeight, int.MaxValue);

            Assert.True(sut.MazeMap != null);
            
        }

        /// <summary>
        /// Will Retunr a callback from a giver solver that generates a solution
        /// </summary>
        [Fact]
        public void ReturnCallbackFromGivenSolver()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

            sut.Load(loader);


            var slogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<BFSGraphSolver>();
            BFSGraphSolver solver = new BFSGraphSolver(slogger);

            bool called = false; 

            sut.Solve(solver, (c) => {
                called = true; 
            });
                        
            Assert.True(called);
        }

        [Fact]
        public void GenerateGraph()
        {
            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

            sut.Load(loader);

            Graph<IMazeCell> _graph = sut.GenerateGraph();

            Assert.True(_graph.Nodes.Count() > 0);

        }


        [Fact]
        public void ThrowOnGenerateGraphIfGridIsEmpty()
        {
            
            var mlogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();
            var xsut = new MazeGrid(mlogger);

            Assert.Throws<ArgumentNullException>(() => {
                xsut.GenerateGraph();
            });
                        
        }

        [Fact]
        public void ReturnNodeByPosition()
        {
            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

            sut.Load(loader);
            
            // start node
            var node = sut.GetNode(sut.Start.Row, sut.Start.Col); 

            Assert.True( node.Equals(sut.Start));

        }

        [Fact]
        public void ThrowOnGetNodeByPositionIfOutOfRange()
        {
            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader loader = new MazeTextLoader(settings, logger);

            sut.Load(loader);
            
            // out of range node
            Assert.Throws<ArgumentException>(() => {
                var node = sut.GetNode(sut.Height, sut.Width);
            });
                      
        }
        
    }

}
