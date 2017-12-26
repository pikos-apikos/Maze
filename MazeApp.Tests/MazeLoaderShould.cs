using System;
using Xunit;
using MazeApp.Loaders;
using MazeApp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MazeApp.Interfaces;
using MazeApp.Maze;

namespace MazeApp.Tests
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

            sut.Load(grid);
                      

            Assert.InRange<int>(grid.Width, settings.MinWidth, int.MaxValue);
            Assert.InRange<int>(grid.Height, settings.MinHeight, int.MaxValue);
            
            Assert.True(grid.MazeMap != null);

        }

        /// <summary>
        /// Loader should throw if file is not .txt
        /// </summary>
        [Fact]
        public void ThrowIfFileIsNotText()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            settings.MazeFile = "./maze8x8.maz";

            MazeTextLoader sut = new MazeTextLoader(settings, logger);

            var glogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            IMazeGrid grid = new MazeGrid(glogger);

            Assert.Throws<Exception>(() => {
                grid.Load(sut);
            });

        }

        /// <summary>
        /// Loader should throw if file not found
        /// </summary>
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

        /// <summary>
        /// Loader should throw if file is missing start or end point
        /// </summary>
        [Fact]
        public void ThrowIfFileIsMissingStartOrEndPoint()
        {

            var settings = servicesProvider.GetRequiredService<MazeSettings>();

            settings.MazeFile = "./maze8x6-error-no-start.txt";

            MazeTextLoader sut = new MazeTextLoader(settings, logger);

            var glogger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<MazeGrid>();

            IMazeGrid grid = new MazeGrid(glogger);
            
            Assert.Throws<Exception>(() => {
                grid.Load(sut);
            });

        }


    }
}
