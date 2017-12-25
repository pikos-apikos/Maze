/// <summary>
/// Graph library from https://msdn.microsoft.com/en-us/library/ms379574(v=vs.80).aspx
/// this is an abstruct node 
/// </summary>
namespace GraphsLibrary
{
    public class Node<T>
    {
        // Private member-variables
        private T data;
        private NodeList<T> neighbors = null;

        public Node() { }
        public Node(T data) : this(data, null) { }
        public Node(T data, NodeList<T> neighbors)
        {
            this.data = data;
            this.neighbors = neighbors;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        protected NodeList<T> Neighbors
        {
            get
            {
                return neighbors;
            }
            set
            {
                neighbors = value;
            }
        }
    }
}

