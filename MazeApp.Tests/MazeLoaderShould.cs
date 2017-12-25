using System;
using Xunit;
using MazeApp.Loaders;
using MazeApp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MazeApp.Interfaces;
using MazeApp.Maze;

namespace MazeUnitTestProject
{
    public class MazeLoaderShould
    {


        IServiceProvider servicesProvider;
        ILogger<MazeTextLoader> logger;

        public MazeLoaderShould()
        {
            // Load DI
            servicesProvider = MazeApp.Program.BuildDI(null, null);
            logger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeTextLoader>();
        }

        /// <summary>
        /// Using the default configuration. Loader should be able to generate the grid without exceptions
        /// </summary>
        [Fact]
        public void LoadMazeGridFromFile()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            MazeTextLoader sut = new MazeTextLoader(settings, logger);

            var glogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            IMazeGrid grid = new MazeGrid(glogger);

            grid.Load(sut);

            Assert.InRange<int>(grid.Width, settings.MinWidth, int.MaxValue);
            Assert.InRange<int>(grid.Height, settings.MinHeight, int.MaxValue);
            
            Assert.True(grid.MazeMap != null);

        }

        [Fact]
        public void ThrowIfFileNotFound()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            settings.MazeFile = "./I-do-not-exist.txt";

            MazeTextLoader sut = new MazeTextLoader(settings, logger);

            var glogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            IMazeGrid grid = new MazeGrid(glogger);

            
            Assert.Throws<Exception>(() => {
                grid.Load(sut);
            });
            
        }


    }
}
