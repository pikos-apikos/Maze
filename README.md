# Maze App
This is a maze solver app for the e-Travel Maze Challenge

## Build
dotnet restore

dotnet build

## Publish

cd \MazeApp

dotnet publish -c Release -r win10-x64

This will create an executable MazeApp.exe and you will be able to configure through command line and/or the configuration file 

### Command line options


Options:

-  -?|-h|--help          Show help information  
-  -v|--version          Show version information
-  -f|--file <file>      Set path of maze file  
-  -s|--solver <solver>  Set algorithm (supported algorithm BFS, BFSR (Recursive), DFS, DFSR (Recursive), DJK (Dijkstra))
  
