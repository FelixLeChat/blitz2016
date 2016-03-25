using Coveo.Bot;
using Coveo.Core;

namespace Coveo.StateMachine
{
    /// <summary>
    /// Locate the nearest tavern and go use it
    /// </summary>
    public class GoHeal : State
    {
        public override State CalculateNextState(GameState state, TestBot bot)
        {
            // Done Healing or no Cash
            if (state.MyHero.Life > 85 || state.MyHero.Gold == 0)
            {
                // Check if there is a good enemy to steal
                var maxMines = 0;
                foreach (var hero in state.Heroes)
                {
                    if (maxMines < hero.MineCount)
                        maxMines = hero.MineCount;
                }

                // Go steal if he has more mine than you
                if (state.MyHero.MineCount + 3 <= maxMines)
                {
                    return new AttackWinner();
                }

                // No worthy opponent, go capture mines
                return new CaptureMine();
            }

            // Go Heal
            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            // Go to tavern
            return bot.GetClosestTavern(state.MyHero.Pos);
        }
    }
}