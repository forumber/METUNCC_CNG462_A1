using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CNG462_A1___Csharp
{
    class UCS
    {
        // Got and modified from https://medium.com/@basilin/priority-queue-with-c-7089f4898c8d
        class PriorityQueue<TEntry> where TEntry : Node
        {
            public LinkedList<TEntry> Entries { get; } = new LinkedList<TEntry>();

            public int Count()
            {
                return Entries.Count;
            }

            public TEntry Dequeue()
            {
                if (Entries.Any())
                {
                    TEntry ItemTobeRemoved = Entries.First.Value;
                    Entries.RemoveFirst();
                    return ItemTobeRemoved;
                }

                return default;
            }

            public void Enqueue(TEntry Entry)
            {
                LinkedListNode<TEntry> Value = new LinkedListNode<TEntry>(Entry);
                if (Entries.First == null)
                {
                    Entries.AddFirst(Value);
                }
                else
                {
                    LinkedListNode<TEntry> ptr = Entries.First;
                    while (ptr.Next != null && ptr.Value.g < Entry.g)
                    {
                        ptr = ptr.Next;
                    }

                    if (ptr.Value.g <= Entry.g)
                    {
                        Entries.AddAfter(ptr, Value);
                    }
                    else
                    {
                        Entries.AddBefore(ptr, Value);
                    }
                }
            }
        }

        class Node
        {
            public Node Parent;
            public List<Node> Childs;
            public string Path;
            public int g;
            public string NodeName;

            public Node(string NodeName, int g)
            {
                this.NodeName = NodeName;
                this.g = g;

                this.Childs = new List<Node>();
                this.Parent = null;
                this.Path = NodeName;
            }

            public void SetParent(Node Parent)
            {
                this.Parent = Parent;
                this.g = Parent.g + this.g;
                this.Path = this.Parent.Path + this.NodeName;
                this.Parent.Childs.Add(this);
            }
        }

        readonly Dictionary<Tuple<string, string>, int> ShortestPaths;
        List<string> Points;
        PriorityQueue<Node> MainQueue;
        readonly int TargetPointAmount;
        int NodeCount;

        public UCS(Dictionary<Tuple<string, string>, int> ShortestPaths)
        {
            this.ShortestPaths = ShortestPaths;
            this.MainQueue = new PriorityQueue<Node>();
            this.Points = new List<string>();
            this.TargetPointAmount = Convert.ToInt32(Math.Floor(Math.Sqrt(this.ShortestPaths.Count * 2)));
            NodeCount = 0;

            foreach (var Path in ShortestPaths.Keys)
                if (!Points.Contains(Path.Item1))
                    Points.Add(Path.Item1);
        }

        private void GenerateSuccessors(Node Q)
        {
            foreach (var PointTo in Points)
            {

                if (!(PointTo == Q.Path.First().ToString() && Q.Path.Length == TargetPointAmount))
                    if (Q.Path.Contains(PointTo))
                        continue;

                Node TempNode = new Node(PointTo, ShortestPaths[Tuple.Create(Q.NodeName, PointTo)]);

                TempNode.SetParent(Q);
            }
        }

        public Tuple<string, int> Search(string MainCP)
        {
            var StartingNode = new Node(MainCP, 0);

            MainQueue.Enqueue(StartingNode);

            while (MainQueue.Count() != 0)
            {
                NodeCount++;

                Node Q = MainQueue.Dequeue();
                GenerateSuccessors(Q);

                foreach (Node Successor in Q.Childs)
                {
                    if (Successor.NodeName == MainCP)
                        return Tuple.Create(Successor.Path, Successor.g);

                    MainQueue.Enqueue(Successor);
                }
            }

            return null; // notfound
        }

        public int GetNodeCount()
        {
            return NodeCount;
        }
    }
}
