# Maze App

This is a maze solver app for the e-Travel Maze Challenge written in C# using .NET Core 2.0 

## Build

dotnet restore

dotnet build

## Create an executable

cd \MazeApp

dotnet publish -c Release -r win10-x64

This will create an executable MazeApp.exe that you will be able to configure through command line and/or the configuration file 

### Command line options

Options:

-  -?|-h|--help          Show help information  
-  -v|--version          Show version information
-  -f|--file <file>      Set path of maze file  
-  -s|--solver <solver>  Set algorithm (supported algorithm BFS, BFSR (Recursive), DFS, DFSR (Recursive), DJK (Dijkstra))
  
### Configuration File options (mazesettings.json)

-  MazeFile: Maze file path
-  MazeSolver: Algorithm (supported algorithm BFS, BFSR (Recursive), DFS, DFSR (Recursive), DJK (Dijkstra))
-  OpenChar: Open path character, default: _
-  WallChar: Wall character, default: X
-  StartChar: Start character, default: S
-  FinishChar: End character, default: G

### Running

Running the App using MazeApp.exe or dotnet run, will report the results in the terminal and will produce a log file in the .\Logs folder with any Info, Errors or Warning produced by the app
