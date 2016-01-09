﻿// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Coveo.Bot;

namespace CoveoBlitz.RandomBot
{
    /// <summary>
    /// RandomBot
    ///
    /// This bot will randomly chose a direction each turns.
    /// </summary>
    public class RandomBot : ISimpleBot
    {
        private readonly Random random = new Random();

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
            return CalculatePath(state, state.heroes[1].pos);
        }

        private string CalculatePath(GameState state, Pos goal)
        {
            Console.WriteLine("Begining calculating path");
            Pos start = state.myHero.pos;

            try
            {
                List<Pos> visited = new List<Pos>();
                List<PathCoord> availableTiles = new List<PathCoord>();

                var first = createPathCoord(start, null, state, goal, null);
                availableTiles.Add(first);

                while (availableTiles.Any())
                {
                    // Sort availableTiles
                    availableTiles.Sort((f1, f2) => f1.heuristic.CompareTo(f2.heuristic));

                    var currentVisited = availableTiles.First();
                    availableTiles.Remove(currentVisited);

                    if (visited.Contains(currentVisited.current))
                    {
                        continue;
                    }

                    visited.Add(currentVisited.current);

                    if (currentVisited.current == goal)
                    {
                        Console.WriteLine("Path found!");
                        return Restitute(start, currentVisited);
                    }
                    else
                    {
                        var spreadFrom = getAvailableCoords(currentVisited, state, goal);
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
                if (getDistance(start, foundPath.current) == 1)
                {
                    return foundPath.previousDirection;
                }
                foundPath = foundPath.previous;
            }

            Console.WriteLine("Error with restitute");
            return Direction.Stay;
        }


        private List<PathCoord> getAvailableCoords(PathCoord current, GameState state, Pos goal)
        {
            PathCoord east = createPathCoord(new Pos() { x = current.current.x, y = current.current.y + 1 }, current, state,
                goal, Direction.East);
            PathCoord west = createPathCoord(new Pos() { x = current.current.x, y = current.current.y - 1 }, current, state,
                goal, Direction.West);
            PathCoord north = createPathCoord(new Pos() { x = current.current.x - 1, y = current.current.y }, current, state,
                goal, Direction.North);
            PathCoord south = createPathCoord(new Pos() { x = current.current.x + 1, y = current.current.y }, current, state,
                goal, Direction.South);

            var rc = new List<PathCoord>();
            if (east != null && isValid(state, east))
            {
                rc.Add(east);
            }
            if (west != null && isValid(state, west))
            {
                rc.Add(west);
            }
            if (north != null && isValid(state, north))
            {
                rc.Add(north);
            }
            if (south != null && isValid(state, south))
            {
                rc.Add(south);
            }
            return rc;
        }

        private bool isValid(GameState state, PathCoord coordToValidate)
        {
            if (coordToValidate.current.x < 0 || coordToValidate.current.x >= state.board.GetLength(0) ||
                coordToValidate.current.y < 0 || coordToValidate.current.y >= state.board.GetLength(1))
            {
                return false;
            }

            return true;
        }

        private PathCoord createPathCoord(Pos current, PathCoord previous, GameState state, Pos goal, string previousDirection)
        {
            PathCoord pc = new PathCoord();

            if (state.board[current.x][current.y] == Tile.IMPASSABLE_WOOD)
            {
                return null;
            }

            if (previous != null)
            {
                pc.weight = previous.weight; // to modify
            }

            if (state.board[current.x][current.y] == Tile.SPIKES)
            {
                pc.weight += 3; // to modify
            }
            else
            {
                pc.weight += 1;
            }
            pc.current = current;
            pc.previous = previous;
            pc.previousDirection = previousDirection;

            pc.heuristic = pc.weight + getDistance(current, goal);
            return pc;
        }

        private int getDistance(Pos a, Pos b)
        {
            return Math.Abs(b.x - a.x) + Math.Abs(b.y - a.y);
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