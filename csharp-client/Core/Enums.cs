namespace Coveo.Core
{
    public enum Tile
    {
        ImpassableWood,
        Free,
        Spikes,
        Hero1,
        Hero2,
        Hero3,
        Hero4,
        Tavern,
        GoldMineNeutral,
        GoldMine1,
        GoldMine2,
        GoldMine3,
        GoldMine4
    }

    public class Direction
    {
        public const string Stay = "Stay";
        public const string North = "North";
        public const string East = "East";
        public const string South = "South";
        public const string West = "West";
    }

    public class Constant
    {
        public const int LifeDrainOnHit = 25;
    }

    public static class Extensions
    {
        public static Tile At(this Tile[][] tiles, Pos pos)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X > tiles.Length || pos.Y > tiles[0].Length)
                return Tile.ImpassableWood;
            return tiles[pos.Y][pos.X];
        }
    }
}