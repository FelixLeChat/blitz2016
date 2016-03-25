using System;
using System.Linq;
using Coveo.Bot;
using Coveo.Core;

namespace Coveo.StateMachine
{
    /// <summary>
    /// State where we search for the next mine to get
    /// </summary>
    public class CaptureMine : State
    {
        public override State CalculateNextState(GameState state, TestBot bot)
        {
            // if capturing a mine will kill you
            if(state.MyHero.Life < 26)
                return new GoHeal();

            // Max mine of hero
            var maxMines = state.Heroes.Select(hero => hero.MineCount).Concat(new[] {0}).Max();

            // If we have 3 mine less than the max mine of player
            if (state.MyHero.MineCount + 3 <= maxMines)
            {
                if(state.MyHero.Life >= 75)
                    return new AttackWinner();
                
                return new GoHeal();
            }

            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            Console.WriteLine("Capturing mine");
            return bot.GetClosestMine(state.MyHero.Pos, state.Board);
        }
    }
}