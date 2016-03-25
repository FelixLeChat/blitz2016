using System.Collections.Generic;

namespace Coveo.Core
{
    public class GameState
    {
        public Hero MyHero { get; set; }
        public List<Hero> Heroes { get; set; }

        public int CurrentTurn { get; set; }
        public int MaxTurns { get; set; }
        public bool Finished { get; set; }
        public bool Errored { get; set; }

        public Tile[][] Board { get; set; }
    }
}