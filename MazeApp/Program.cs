using MazeApp.Exceptions;
using MazeApp.Maze;
using MazeApp.Solvers;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MazeApp
{
    public class Program
    {
        /*
        * Configuration Options 
        * 
        * [-?|-h|--help] Display Help, 
        * [-f|--file] Load a maze file
        * [-a|--algo] Choose an algorithm for solving
        * 
        * Credits: 
        *   (Ο Λαβύρινθος του Θησέα και ο κανόνας δεξιού χεριού)
        *   https://github.com/mikepound/mazesolving/blob/master/dijkstra.py
        */

        static void Main(string[] args)
        {

            // Step 1: Configuration 

            // Instantiate the command line app
            var app = new CommandLineApplication
            {
                Name = "MazeApp",
                Description = ".NET Core console app that solves mazes using (i hope) Dijkstra, A*, Depth-First Search and Breadth-First Search.",
                ExtendedHelpText = "This is a demo console app for e-Travel Maze Challenge"
            };

            // Set the arguments to display the description and help text
            app.HelpOption("-?|-h|--help");

            // This is a helper/shortcut method to display version info
            app.VersionOption("-v|--version", () => {
                return string.Format("Version {0}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            });

            // Set argument to override MazeFile setting 
            var fileOption = app.Option("-f|--file <file>", "Set path of maze file", CommandOptionType.SingleValue);

            // Set argument to override MazeFile setting 
            var solverOption = app.Option("-s|--solver <solver>", "Set algorithm (supported algorithm BFS, BFSR (Recursive), DFS, DFSR (Recursive), DJK (Dijkstra))", CommandOptionType.SingleValue);

            // @todo: no-time
            // var loader = app.Option("-l|--loader <loader>", "supported loader Text(default), Image", CommandOptionType.SingleValue);


            // When no commands are specified, this block will execute.
            app.OnExecute(() =>
            {

                // Create service collection dependency injection
                // *** overkill :)
                var servicesProvider = BuildDI(fileOption, solverOption);
                var logger = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
                var settings = servicesProvider.GetRequiredService<MazeSettings>();                
                var runner = servicesProvider.GetRequiredService<Interfaces.IMazeGrid>();
                var loader = servicesProvider.GetRequiredService<Interfaces.IMazeLoader>();
                               

                // Step 2: Load a maze 
                runner.Load(loader);

                // Step 3: Use BFS to check solvability
                var solutions = new List<IEnumerable<Interfaces.IMazeCell>>();
                runner.Solve(new BFSGraphSolver(
                    servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<BFSGraphSolver>()), 
                    (solvedPath) =>
                {
                    if (solvedPath.Count() > 0)
                    {
                        solutions.Add(solvedPath);                            
                    }

                });


                // Step 4: Solve the maze 
                if (solutions.Count > 0)
                {
                    // reset
                    solutions.Clear();

                    var solver = servicesProvider.GetRequiredService<Interfaces.IMazeSolver>();

                    runner.Solve(solver, (solvedPath) =>
                    {

                        if (solvedPath.Count() > 0)
                        {
                            solutions.Add(solvedPath);
                        }

                    });

                    // Step 5: Report 
                    if (solutions.Count() > 0)
                    {

                        // Report solution ASC
                        solutions.Sort((a, b) => b.Count() - a.Count());

                        logger.LogInformation($"Found Solution(s): {solutions.Count()})");


                        foreach (var solvedPath in solutions)
                        {

                            var sb = new StringBuilder();

                            foreach (var node in solvedPath)
                            {

                                if (sb.Length > 0)
                                    sb.Append(", ");

                                if (runner.Start.Equals(node))
                                    sb.Append($"({node.ToString()} ({settings.StartChar}))");
                                else if (runner.Finish.Equals(node))
                                    sb.Append($"({node.ToString()} ({settings.FinishChar}))");
                                else
                                    sb.Append(node.ToString());

                            }

                            logger.LogInformation(sb.ToString());

                        }


                    }
                    else
                    {
                        logger.LogCritical("No solutions where found for this solvable maze, check algo!");
                    }

                }
                else
                {
                    logger.LogCritical("This maze is unsolvable");
                }
                                
                return 0;
            });



            try
            {
                // This begins the actual execution of the application
                Console.WriteLine("ConsoleArgs app executing...");

                app.Execute(args);

                Console.WriteLine("Press any to exit");
                Console.ReadLine();

            }
            catch (CommandParsingException ex)
            {
                // You'll always want to catch this exception, otherwise it will generate a messy and confusing error for the end user.
                // the message will usually be something like:
                // "Unrecognized command or argument '<invalid-command>'"
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to execute application: {0}", ex.Message);
            }
                       
        }

        /// <summary>
        /// Build dependency injection
        /// </summary>
        /// <param name="fileOption"></param>
        /// <param name="solverOption"></param>
        /// <returns></returns>
        public static IServiceProvider BuildDI(CommandOption fileOption, CommandOption solverOption)
        {

            

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("mazesettings.json", false)
                .Build();

            var services = new ServiceCollection();

            // Add logging
            var logger = new LoggerFactory()
                .AddFile("Logs/mazeapp-{Date}.log")
                .AddConsole()
                .AddDebug(minLevel: LogLevel.Trace); 
            
            services.AddSingleton(logger).AddLogging();


            // Load settings
            var settings = new MazeSettings();

            configuration.GetSection("Maze").Bind(settings);

            if ((fileOption != null) && fileOption.HasValue())
            {
                settings.MazeFile = fileOption.Value();
            }

            if ((solverOption != null) && solverOption.HasValue())
            {
                settings.MazeSolver = solverOption.Value();
            }

            try
            {
                                
                // validate settings
                Validations.ValidateConfiguration(settings);
            }
            catch (InvalidConfigurationParameterException e)
            {

                logger.CreateLogger<Program>().LogError(e, e.Message);
                throw;
            }

            // Add settings            
            services.AddSingleton<MazeSettings>(settings);

            // Add runner class
            services.AddTransient<Interfaces.IMazeGrid, Maze.MazeGrid>();

            // Add loader class
            services.AddTransient<Interfaces.IMazeLoader, Loaders.MazeTextLoader>();

            
            // Add solver classes             
            switch (settings.MazeSolver)
            {
                case "BFS":
                    services.AddTransient<Interfaces.IMazeSolver, BFSGraphSolver>();
                    break;

                case "BFSR":
                    services.AddTransient<Interfaces.IMazeSolver, BFSRecursiveGraphSolver>();
                    break;

                case "DFS":
                    services.AddTransient<Interfaces.IMazeSolver, DFSGraphSolver>();
                    break;

                case "DFSR":
                    services.AddTransient<Interfaces.IMazeSolver, DFSRecursiveGraphSolver>();
                    break;

                case "DJK":
                    services.AddTransient<Interfaces.IMazeSolver, DijkstraGraphSolver>();
                    break;

                default:
                    services.AddTransient<Interfaces.IMazeSolver, BFSGraphSolver>();
                    break;
            }

            // register a factory to return a solver from settings
            

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

        
    }
}
