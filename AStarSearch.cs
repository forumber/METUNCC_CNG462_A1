﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CNG462_A1___Csharp
{
    class AStarSearch
    {
        class Node
        {
            public Node Parent;
            public List<Node> Childs;
            public int f, g, h;
            public string NodeName;
            public Tuple<int, int> Position;

            public Node(string NodeName, Tuple<int, int> Position, int h, int g = 0)
            {
                this.NodeName = NodeName;
                this.Position = Position;
                this.g = g; this.h = h;
                this.f = this.g + this.h;

                this.Childs = new List<Node>();
                this.Parent = null;
            }

            public void SetParent(Node Parent)
            {
                this.Parent = Parent;
                this.g = Parent.g + 1;
                this.Parent.Childs.Add(this);
            }
        }

        List<Node> OpenList;
        List<Node> ClosedList;
        readonly Dictionary<string, Tuple<int, int>> AvailableDirectionToGo;
        readonly Dictionary<string, Tuple<int, int>> PositionOfPoints;
        readonly char[][] mainArray;
        readonly string Start;
        readonly string Destination;

        public AStarSearch(string Start, string Destination, char[][] mainArray, Dictionary<string, Tuple<int, int>> PositionOfPoints)
        {
            OpenList = new List<Node>();
            ClosedList = new List<Node>();

            AvailableDirectionToGo = new Dictionary<string, Tuple<int, int>>
            {
                ["Right"] = Tuple.Create(0, 1),
                ["Down"] = Tuple.Create(1, 0),
                ["Left"] = Tuple.Create(0, -1),
                ["Up"] = Tuple.Create(-1, 0)
            };

            this.mainArray = mainArray;
            this.PositionOfPoints = PositionOfPoints;
            this.Start = Start;
            this.Destination = Destination;
        }

        private Node FindNodeWithMinimumF()
        {
            Node MinF = OpenList.First();

            foreach (Node TheNode in OpenList)
            {
                if (MinF.f > TheNode.f)
                    MinF = TheNode;
            }

            OpenList.Remove(MinF);
            return MinF;

        }

        private static int GetManhattanDistance(Tuple<int, int> Position1, Tuple<int, int> Position2)
        {
            return Math.Abs(Position1.Item1 - Position2.Item1) + Math.Abs(Position1.Item2 - Position2.Item2) - 1;
        }

        private void GenerateSuccessors(Node Q)
        {

            foreach (var Direction in AvailableDirectionToGo.Keys)
            {
                var DirectionPos = AvailableDirectionToGo[Direction];

                var NewPos = Tuple.Create(Q.Position.Item1 + DirectionPos.Item1, Q.Position.Item2 + DirectionPos.Item2);

                string PositionName = PositionOfPoints.FirstOrDefault(x => x.Value.Equals(NewPos)).Key;

                if (PositionName != null)
                {
                    Node TempNode = new Node(PositionName, PositionOfPoints[PositionName],
                    h: GetManhattanDistance(PositionOfPoints[PositionName], PositionOfPoints[Destination]));

                    TempNode.SetParent(Q);
                }
            }
        }

        public int Search()
        {
            var StartingNode = new Node(Start, PositionOfPoints[Start],
                h: GetManhattanDistance(PositionOfPoints[Start], PositionOfPoints[Destination]));

            OpenList.Add(StartingNode);

            while (OpenList.Any())
            {
                Node Q = FindNodeWithMinimumF();
                GenerateSuccessors(Q);

                foreach (Node Successor in Q.Childs)
                {
                    if (Successor.NodeName == Destination)
                    {
                        Node CurrentNode = Successor;
                        int Distance = 0;

                        while (CurrentNode != StartingNode)
                        {
                            CurrentNode = CurrentNode.Parent;
                            Distance++;
                        }

                        return Distance;
                    }
                        

                    bool SkipThisSuccessor = false;

                    foreach (Node OpenNode in OpenList)
                        if ((OpenNode.NodeName == Successor.NodeName) && (OpenNode.f < Successor.f))
                        {
                            SkipThisSuccessor = true;
                            break;
                        }

                    if (SkipThisSuccessor)
                        continue;

                    foreach (Node ClosedNode in ClosedList)
                        if ((ClosedNode.NodeName == Successor.NodeName) && (ClosedNode.f < Successor.f))
                        {
                            SkipThisSuccessor = true;
                            break;
                        }

                    if (SkipThisSuccessor)
                        continue;

                    OpenList.Add(Successor);
                }

                ClosedList.Add(Q);
            }

            return -1; //error
        }
    }
}
