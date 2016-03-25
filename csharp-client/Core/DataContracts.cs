using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Coveo.Core
{
    [DataContract]
    public class GameResponse
    {
        [DataMember]
        public Game Game;

        [DataMember]
        public Hero Hero;

        [DataMember]
        public string Token;

        [DataMember]
        public string ViewUrl;

        [DataMember]
        public string PlayUrl;
    }

    [DataContract]
    public class Game
    {
        [DataMember]
        public string Id;

        [DataMember]
        public int Turn;

        [DataMember]
        public int MaxTurns;

        [DataMember]
        public List<Hero> Heroes;

        [DataMember]
        public Board Board;

        [DataMember]
        public bool Finished;
    }

    [DataContract]
    public class Hero
    {
        [DataMember]
        public int Id;

        [DataMember]
        public string Name;

        [DataMember]
        public int Elo;

        [DataMember]
        public Pos Pos;

        [DataMember]
        public int Life;

        [DataMember]
        public int Gold;

        [DataMember]
        public int MineCount;

        [DataMember]
        public Pos SpawnPos;

        [DataMember]
        public bool Crashed;
    }

    [DataContract]
    public class Pos
    {
        [DataMember]
        public int X;

        [DataMember]
        public int Y;
    }

    [DataContract]
    public class Board
    {
        [DataMember]
        public int Size;

        [DataMember]
        public string Tiles;
    }
}