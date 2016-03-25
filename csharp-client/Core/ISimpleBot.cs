namespace Coveo.Core
{
    public interface ISimpleBot
    {
        void Setup();

        void Shutdown();

        string Move(GameState state);
    }
}