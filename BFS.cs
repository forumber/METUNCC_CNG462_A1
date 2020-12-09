using System;
using System.Collections.Generic;
using System.Linq;

namespace CNG462_A1___Csharp
{
    class BFS
    {
        readonly Dictionary<Tuple<string, string>, int> ShortestPaths;
        Queue<string> MainQueue;
        readonly int TargetPointAmount;
        int NodeCount;

        public BFS(Dictionary<Tuple<string, string>, int> ShortestPaths)
        {
            this.ShortestPaths = ShortestPaths;
            MainQueue = new Queue<string>();
            TargetPointAmount = Convert.ToInt32(Math.Floor(Math.Sqrt(this.ShortestPaths.Count * 2)));
            NodeCount = 0;
        }

        private int GetDistanceOfPath(string Path)
        {
            int DistanceSoFar = 0;

            char[] PathArray = Path.ToCharArray();

            for (int i = 0; i < PathArray.Length - 1; i++)
                DistanceSoFar += ShortestPaths[Tuple.Create(PathArray[i].ToString(), PathArray[i + 1].ToString())];

            return DistanceSoFar;
        }

        public Tuple<string, int> Search(string MainCP)
        {
            MainQueue.Enqueue(MainCP);

            while(MainQueue.Count != 0)
            {
                NodeCount++;

                string Path = MainQueue.Dequeue();
                string CurrentlyLooking = Path.Last().ToString();

                foreach (Tuple<string, string> NodeToNode in ShortestPaths.Keys)
                    if (NodeToNode.Item1 == CurrentlyLooking)
                    {
                        if (Path.Length == TargetPointAmount && NodeToNode.Item2 == MainCP)
                            return Tuple.Create(Path + NodeToNode.Item2, GetDistanceOfPath(Path + NodeToNode.Item2));

                        if (!Path.Contains(NodeToNode.Item2))
                            MainQueue.Enqueue(Path + NodeToNode.Item2);
                    } 
            }

            return null; // not found
        }

        public int GetNodeCount()
        {
            return NodeCount;
        }

    }
}
