using Coveo.Bot;
using Coveo.Core;

namespace Coveo.StateMachine
{
    public abstract class State
    {
        public virtual State CalculateNextState(GameState state, TestBot bot)
        {
            return this;
        }

        public abstract Pos GetGoal(GameState state, TestBot bot);
    }
}