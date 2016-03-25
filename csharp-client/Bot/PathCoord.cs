using Coveo.Core;

namespace Coveo.Bot
{
    public class PathCoord
    {
        public Pos Current;
        public PathCoord Previous;

        public int Heuristic;
        public int Weight;

        public string PreviousDirection;
    }
}
