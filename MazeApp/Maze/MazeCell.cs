using MazeApp.Interfaces;

namespace MazeApp.Maze
{
    
    public sealed class MazeCell : IMazeCell
    {
                
        private int _rowPosition, _colPosition;
        private CellState _state;

        /// <summary>
        /// Neighbours by Direction
        /// Clock: 0: Nort ^ , 1: East > , 2: South \/ , 3: West <
        /// </summary>
        private IMazeCell[] _neighbours = new IMazeCell[4];  

        public int Row
        {
            get { return _rowPosition; }
        }

        public int Col
        {
            get { return _colPosition; }
        }

        public CellState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public IMazeCell[] Neighbours
        {
            get
            {
                return _neighbours;
            }
            set
            {
                _neighbours = value;
            }
        }

        public MazeCell(int row, int col)
        {
            _rowPosition = row;
            _colPosition = col;
        }

        public override bool Equals(object obj)
        {
            MazeCell mazeNode = obj as MazeCell;
            if (mazeNode == null)
                return false;
            if (object.ReferenceEquals(mazeNode, this))
                return true;

            // validate possition and state for equality 
            return (mazeNode.Col == this.Col && mazeNode.Row == this.Row && mazeNode.State == this.State);
        }

        public override int GetHashCode()
        {
            //changed according to the implementation of Equals().
            return base.GetHashCode() + Col + Row + (int)State; 
        }

        public override string ToString()
        {
            // override to ease reporting
            return $"({_rowPosition},{_colPosition})";
        }

    }
}
