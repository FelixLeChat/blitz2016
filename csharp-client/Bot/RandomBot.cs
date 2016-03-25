// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using Coveo.Core;

namespace Coveo.Bot
{
    /// <summary>
    /// RandomBot
    ///
    /// This bot will randomly chose a direction each turns.
    /// </summary>
    public class RandomBot : ISimpleBot
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// This will be run before the game starts
        /// </summary>
        public void Setup()
        {
            Console.WriteLine("Coveo's C# RandomBot");
        }

        /// <summary>
        /// This will be run on each turns. It must return a direction fot the bot to follow
        /// </summary>
        /// <param name="state">The game state</param>
        /// <returns></returns>
        public string Move(GameState state)
        {
            return CalculatePath(state, state.Heroes[1].Pos);
        }

        private string CalculatePath(GameState state, Pos goal)
        {
            var start = state.MyHero.Pos;

            Console.WriteLine("Begining calculating path from ({0},{1}) to ({2},{3})", start.X, start.Y, goal.X, goal.Y);

            try
            {
                var visited = new List<Pos>();
                var availableTiles = new List<PathCoord>();

                var first = CreatePathCoord(start, null, state, goal, null);
                availableTiles.Add(first);

                while (availableTiles.Any())
                {
                    // Sort availableTiles
                    availableTiles.Sort((f1, f2) => f1.Heuristic.CompareTo(f2.Heuristic));

                    var currentVisited = availableTiles.First();
                    availableTiles.Remove(currentVisited);

                    if (visited.Any(x => x.X == currentVisited.Current.X && x.Y==currentVisited.Current.Y))
                    {
                        continue;
                    }

                   // Console.WriteLine("Visiting ({0},{1}) with heuristic {2}", currentVisited.current.x, currentVisited.current.y, currentVisited.heuristic);

                    visited.Add(currentVisited.Current);

                    if (GetDistance(currentVisited.Current, goal) == 0)
                    {
                        var direction = Restitute(start, currentVisited);
                        Console.WriteLine("Path found! Going {0}", direction);
                        return direction;
                    }
                    else
                    {
                        var spreadFrom = GetAvailableCoords(currentVisited, state, goal);
                        availableTiles.AddRange(spreadFrom);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("No path found!");
            return Direction.Stay;
        }

        private string Restitute(Pos start, PathCoord foundPath)
        {
            while (foundPath != null)
            {
                var distance = GetDistance(start, foundPath.Current);
                if (distance == 1)
                {
                    return foundPath.PreviousDirection;
                }
                foundPath = foundPath.Previous;
            }

            Console.WriteLine("Error with restitute");
            return Direction.Stay;
        }


        private List<PathCoord> GetAvailableCoords(PathCoord current, GameState state, Pos goal)
        {
            var east = CreatePathCoord(new Pos() { X = current.Current.X, Y = current.Current.Y + 1 }, current, state,
                goal, Direction.East);
            var west = CreatePathCoord(new Pos() { X = current.Current.X, Y = current.Current.Y - 1 }, current, state,
                goal, Direction.West);
            var north = CreatePathCoord(new Pos() { X = current.Current.X - 1, Y = current.Current.Y }, current, state,
                goal, Direction.North);
            var south = CreatePathCoord(new Pos() { X = current.Current.X + 1, Y = current.Current.Y }, current, state,
                goal, Direction.South);

            var rc = new List<PathCoord>();
            if (east != null)
            {
                rc.Add(east);
            }
            if (west != null)
            {
                rc.Add(west);
            }
            if (north != null)
            {
                rc.Add(north);
            }
            if (south != null)
            {
                rc.Add(south);
            }
            return rc;
        }

        private bool IsValid(GameState state, Pos coordToValidate)
        {
            var size = state.Board.GetLength(0);

            if (coordToValidate.X < 0 || coordToValidate.X >= size ||
                coordToValidate.Y < 0 || coordToValidate.Y >= size)
            {
                return false;
            }

            return true;
        }

        private PathCoord CreatePathCoord(Pos current, PathCoord previous, GameState state, Pos goal, string previousDirection)
        {
            try
            {
                if (!IsValid(state, current))
                {
                    return null;
                }

                var pc = new PathCoord();

                if (state.Board[current.Y][current.X] == Tile.ImpassableWood)
                {
                    return null;
                }

                if (previous != null)
                {
                    pc.Weight = previous.Weight; // to modify
                }

                pc.Weight += GetCost(state.Board[current.Y][current.X]);
                pc.Current = current;
                pc.Previous = previous;
                pc.PreviousDirection = previousDirection;

                pc.Heuristic = pc.Weight + GetDistance(current, goal);
                return pc;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error with bound");
                throw e;
            }
        }

        private int GetCost(Tile tile)
        {
            int cost;
            switch (tile)
            {
                case Tile.Spikes:
                    cost = 10;
                    break;
                default:
                    cost = 1;
                    break;
            }
            return cost;
        }

        private int GetDistance(Pos a, Pos b)
        {
            return Math.Abs(b.X - a.X) + Math.Abs(b.Y - a.Y);
        }

        //public string Move(GameState state)
        //{
        //    string direction;

        //    switch (random.Next(0, 5))
        //    {
        //        case 0:
        //            direction = Direction.East;
        //            break;

        //        case 1:
        //            direction = Direction.West;
        //            break;

        //        case 2:
        //            direction = Direction.North;
        //            break;

        //        case 3:
        //            direction = Direction.South;
        //            break;

        //        default:
        //            direction = Direction.Stay;
        //            break;
        //    }

        //    Console.WriteLine("Completed turn {0}, going {1}", state.currentTurn, direction);
        //    return direction;
        //}

        /// <summary>
        /// This is run after the game.
        /// </summary>
        public void Shutdown()
        {
            Console.WriteLine("Done");
        }
    }
}