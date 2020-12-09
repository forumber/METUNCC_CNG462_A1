using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CNG462_A1___Csharp
{
    class Program
    {
        static char[][] mainArray;
        static Dictionary<string, Tuple<int, int>> PositionOfPoints;
        static Dictionary<Tuple<string, string>, int> ShortestPaths;
        static BFS bFS;
        static UCS uCS;
        static Tuple<string, int> BFSResult;
        static Tuple<string, int> UCSResult;
        static long BFSResultElapsedTime, UCSResultElapsedTime;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: CNG462_A1 <operation> <filename>");
                Console.WriteLine("Available operations: shortestpath, solvetsp");
                Environment.Exit(1);
            }

            string[] txtFileContent = null;

            try
            {
                txtFileContent = System.IO.File.ReadAllLines(args[1]);
            } 
            catch (FileNotFoundException)
            {
                Console.WriteLine("File " + args[1] + " not found!");
                Environment.Exit(1);
            }

            mainArray = txtFileContent.Select(item => item.ToArray()).ToArray();

            PositionOfPoints = new Dictionary<string, Tuple<int, int>>();
            ShortestPaths = new Dictionary<Tuple<string, string>, int>();

            FillPositionOfPoints();

            if (args[0] == "shortestpath")
            {
                FindShortestPaths();
                PrintShortestPaths();
            } 
            else if (args[0] == "solvetsp")
            {
                FindShortestPaths();
                var BFSWatch = System.Diagnostics.Stopwatch.StartNew();
                bFS = new BFS(ShortestPaths);
                BFSResult = bFS.Search("A");
                BFSWatch.Stop();
                var UCSWatch = System.Diagnostics.Stopwatch.StartNew();
                uCS = new UCS(ShortestPaths);
                UCSResult = uCS.Search("A");
                UCSWatch.Stop();

                BFSResultElapsedTime = BFSWatch.ElapsedTicks * 100; // 1 tick = 100 ns
                UCSResultElapsedTime = UCSWatch.ElapsedTicks * 100; // 1 tick = 100 ns

                PrintSolveTSP();
            } 
            else
            {
                Console.WriteLine("Error: Wrong <operation> field!");
                Environment.Exit(1);
            }

        }

        static void PrintSolveTSP()
        {
            Console.WriteLine("Algorithm Used: BFS");
            Console.WriteLine(BFSResult.Item1);
            Console.WriteLine("Total Tour Cost: " + BFSResult.Item2.ToString());
            Console.WriteLine();
            Console.WriteLine("Algorithm Used: UCS");
            Console.WriteLine(UCSResult.Item1);
            Console.WriteLine("Total Tour Cost: " + UCSResult.Item2.ToString());
            Console.WriteLine();
            Console.WriteLine("Statistics:");
            Console.WriteLine("BFS Algorithm; Nodes: " + bFS.GetNodeCount() + ", Time: " + BFSResultElapsedTime.ToString() + " ns, Cost: " + BFSResult.Item2.ToString());
            Console.WriteLine("UCS Algorithm; Nodes: " + uCS.GetNodeCount() + ", Time: " + UCSResultElapsedTime.ToString() + " ns, Cost: " + UCSResult.Item2.ToString());
        }

        static void PrintShortestPaths()
        {
            foreach (var Key in ShortestPaths.Keys)
            {
                if (Key.Item1.Last() < Key.Item2.Last())
                    Console.WriteLine(Key.Item1 + "," + Key.Item2 + "," + ShortestPaths[Key]);
            }
        }

        static void FindShortestPaths()
        {
            foreach (string From in PositionOfPoints.Keys)
            {
                if (From.StartsWith("_"))
                    continue;

                foreach (string To in PositionOfPoints.Keys)
                {
                    if (To.StartsWith("_"))
                        continue;

                    if (From == To)
                        continue;

                    ShortestPaths.Add(Tuple.Create(From, To), new AStarSearch(From, To, mainArray, PositionOfPoints).Search());
                }
            }
        }

        static void FillPositionOfPoints()
        {
            string PointString = "_P";
            int PointNumber = 1;
            var PositionOfPointsUnSorted = new Dictionary<string, Tuple<int, int>>();

            for (int i = 1; i < mainArray.Length - 1; i++)
            {
                for (int j = 1; j < mainArray[i].Length - 1; j++)
                {
                    if (mainArray[i][j] != '*')
                    {
                        if (mainArray[i][j] != ' ')
                            PositionOfPointsUnSorted.Add(mainArray[i][j].ToString(), Tuple.Create(i, j));
                        else
                            PositionOfPointsUnSorted.Add(PointString + (PointNumber++).ToString(), Tuple.Create(i, j));
                    }
                }
            }

            // Sort the dictionary
            var KeyList = new List<string>(PositionOfPointsUnSorted.Keys);
            var OrderedKeyList = KeyList.Where(t => !t.Contains("_")).OrderBy(t => t).ToList();
            OrderedKeyList.AddRange(KeyList.Where(t => t.Contains("_")).OrderBy(t => t).ToList());

            foreach (string Key in OrderedKeyList)
            {
                PositionOfPoints.Add(Key, PositionOfPointsUnSorted[Key]);
            }
        }
    }
}
