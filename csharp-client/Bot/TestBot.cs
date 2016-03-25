using System;
using System.Collections.Generic;
using System.Linq;
using Coveo.Core;
using Coveo.StateMachine;

namespace Coveo.Bot
{
    public class TestBot : ISimpleBot
    {
        public int CostToTavern { get; set; }
        public int CostToMine { get; set; }

        public int Life { get; set; }
        public int Gold { get; set; }

        public int MyHeroId { get; set; }
        public Tile MyHeroEnum { get; set; }
        public List<Pos> Mines = new List<Pos>();
        public List<Pos> Tavernes = new List<Pos>();

        private bool _setup;

        public void Setup()
        {
        }

        public void Shutdown()
        {
        }


        // Start in capturing mine state
        private State _currentState = new CaptureMine();

        private string ProofOnconcept(GameState state)
        {
            _currentState = _currentState.CalculateNextState(state, this);
            var nextGoal = _currentState.GetGoal(state, this);
            return CalculatePath(state, nextGoal).Item1;
        }

        public string Move(GameState state)
        {
            try
            {
                // Update Info
                Life = state.MyHero.Life;
                Gold = state.MyHero.Gold;
                var pos = state.MyHero.Pos;
                var board = state.Board;

                // Initial setup
                if (!_setup)
                {
                    GetImportantPos(state.Board);
                    MyHeroId = state.MyHero.Id;
                    MyHeroEnum = (Tile) (2 + MyHeroId);
                    _setup = true;
                }

                var north = new Pos {X = pos.X - 1, Y = pos.Y};
                var south = new Pos {X = pos.X + 1, Y = pos.Y};
                var east = new Pos {X = pos.X, Y = pos.Y + 1};
                var west = new Pos {X = pos.X, Y = pos.Y - 1};

                // If enough life to capture mine and we don't have the goal to kill the winner
                if (Life > 25 && !(_currentState is AttackWinner))
                {
                    // If adjacent mine present
                    if (MineToClaim(board.At(north)))
                        return Direction.North;
                    if (MineToClaim(board.At(south)))
                        return Direction.South;
                    if (MineToClaim(board.At(west)))
                        return Direction.West;
                    if (MineToClaim(board.At(east)))
                        return Direction.East;
                }

                // Check for healing near if enough funds and life in less than 65
                if (Gold > 1 && Life < 65)
                {
                    //Check for healing
                    if (board.At(north) == Tile.Tavern)
                        return Direction.North;
                    if (board.At(south) == Tile.Tavern)
                        return Direction.South;
                    if (board.At(west) == Tile.Tavern)
                        return Direction.West;
                    if (board.At(east) == Tile.Tavern)
                        return Direction.East;
                }


                return ProofOnconcept(state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }


        public bool OurMine(Tile tile, int hero)
        {
            var ours = false;

            switch (hero)
            {
                case 1:
                    ours = tile == Tile.GoldMine1;
                    break;
                case 2:
                    ours = tile == Tile.GoldMine2;
                    break;
                case 3:
                    ours = tile == Tile.GoldMine3;
                    break;
                case 4:
                    ours = tile == Tile.GoldMine4;
                    break;
            }

            return ours;
        }

        public bool MineToClaim(Tile tile)
        {
            return (tile == Tile.GoldMine1 || tile == Tile.GoldMine2 || tile == Tile.GoldMine3 ||
                    tile == Tile.GoldMine4 || tile == Tile.GoldMineNeutral) && !OurMine(tile, MyHeroId);
        }

        public void GetImportantPos(Tile[][] board)
        {
            for (var i = 0; i < board.Length; i++)
            {
                for (var j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] >= Tile.GoldMineNeutral && board[i][j] <= Tile.GoldMine4)
                    {
                        Mines.Add(new Pos() {X = j, Y = i});
                    }
                    else if (board[i][j] == Tile.Tavern)
                    {
                        Tavernes.Add(new Pos() {X = j, Y = i});
                    }
                }
            }
        }


        public Pos GetClosestMine(Pos pos, Tile[][] board)
        {
            var meilleurePos = new Pos();
            var meilleureDist = int.MaxValue;

            foreach (var mine in Mines)
            {
                if (MineToClaim(board.At(mine)))
                {
                    var tempDist = GetDistance(pos, mine);
                    if (tempDist < meilleureDist)
                    {
                        meilleureDist = tempDist;
                        meilleurePos = mine;
                    }
                }
            }
            return meilleurePos;
        }

        public Pos GetClosestTavern(Pos pos)
        {
            var meilleurePos = new Pos();
            var meilleureDist = int.MaxValue;

            foreach (var taverne in Tavernes)
            {
                var tempDist = GetDistance(pos, taverne);
                if (tempDist < meilleureDist)
                {
                    meilleureDist = tempDist;
                    meilleurePos = taverne;
                }
            }
            return meilleurePos;
        }


        private Tuple<string, int> CalculatePath(GameState state, Pos goal)
        {
            var start = state.MyHero.Pos;

            //Console.WriteLine("Begining calculating path from ({0},{1}) to ({2},{3})", start.x, start.y, goal.x, goal.y);

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

                    if (visited.Any(x => x.X == currentVisited.Current.X && x.Y == currentVisited.Current.Y))
                    {
                        continue;
                    }

                    // Console.WriteLine("Visiting ({0},{1}) with heuristic {2}", currentVisited.current.x, currentVisited.current.y, currentVisited.heuristic);

                    visited.Add(currentVisited.Current);

                    if (GetDistance(currentVisited.Current, goal) == 0)
                    {
                        var direction = Restitute(start, currentVisited);
                        //Console.WriteLine("Path found! Going {0}", direction);
                        return new Tuple<string, int>(direction, currentVisited.Weight);
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
            return new Tuple<string, int>(Direction.Stay, 0);
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
            var east = CreatePathCoord(new Pos() {X = current.Current.X, Y = current.Current.Y + 1}, current,
                state,
                goal, Direction.East);
            var west = CreatePathCoord(new Pos() {X = current.Current.X, Y = current.Current.Y - 1}, current,
                state,
                goal, Direction.West);
            var north = CreatePathCoord(new Pos() {X = current.Current.X - 1, Y = current.Current.Y}, current,
                state,
                goal, Direction.North);
            var south = CreatePathCoord(new Pos() {X = current.Current.X + 1, Y = current.Current.Y}, current,
                state,
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
            if (coordToValidate.X < 0 || coordToValidate.X > state.Board.Length ||
                coordToValidate.Y < 0 || coordToValidate.Y > state.Board[0].Length)
            {
                return false;
            }

            return true;
        }

        private PathCoord CreatePathCoord(Pos current, PathCoord previous, GameState state, Pos goal,
            string previousDirection)
        {
            try
            {
                if (!IsValid(state, current))
                {
                    return null;
                }

                var pc = new PathCoord();

                var currentTile = state.Board.At(current);

                if (
                    !(currentTile == Tile.Free || currentTile == Tile.Spikes || AreEqual(goal, current) ||
                      (currentTile >= Tile.Hero1 && currentTile <= Tile.Hero4)))
                {
                    return null;
                }

                if (previous != null)
                {
                    pc.Weight = previous.Weight; // to modify
                }

                pc.Weight += GetCost(state, current);
                pc.Current = current;
                pc.Previous = previous;
                pc.PreviousDirection = previousDirection;

                pc.Heuristic = pc.Weight + GetDistance(current, goal);

                // Void path if it would kill us
                if (CanKill(currentTile) && state.MyHero.Life < pc.Weight)
                {
                    return null;
                }

                return pc;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with bound");
                throw e;
            }
        }

        private bool CanKill(Tile tile)
        {
            if (tile == Tile.Spikes || (tile >= Tile.Hero1 && tile <= Tile.Hero4))
            {
                return true;
            }
            return false;
        }

        private int GetCost(GameState state, Pos pos)
        {
            var cost = 0;

            // Check if next to ennemy
            if (IsNextToEnnemy(state, pos))
            {
                cost += 25;
            }

            cost += GetTileCost(state.Board.At(pos));

            return cost;
        }

        private bool IsNextToEnnemy(GameState state, Pos pos)
        {
            return GetAdjacent(state, pos).ToList().Any(currentPos => IsTileEnnemy(state, state.Board.At(currentPos)) && GetDistance(currentPos, pos) < 2);
        }

        private bool IsTileEnnemy(GameState state, Tile tile)
        {
            if (tile == MyHeroEnum)
            {
                return false;
            }
            if (tile >= Tile.Hero1 && tile <= Tile.Hero4)
            {
                return true;
            }
            return false;
        }

        private List<Pos> GetAdjacent(GameState state, Pos pos)
        {
            var list = new List<Pos>();

            var north = new Pos {X = pos.X - 1, Y = pos.Y};
            var south = new Pos {X = pos.X + 1, Y = pos.Y};
            var east = new Pos {X = pos.X, Y = pos.Y + 1};
            var west = new Pos {X = pos.X, Y = pos.Y - 1};

            if (IsValid(state, north))
            {
                list.Add(north);
            }
            if (IsValid(state, south))
            {
                list.Add(south);
            }
            if (IsValid(state, east))
            {
                list.Add(east);
            }
            if (IsValid(state, west))
            {
                list.Add(west);
            }

            return list;
        }

        private int GetTileCost(Tile tile)
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

        private bool AreEqual(Pos a, Pos b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
    }
}