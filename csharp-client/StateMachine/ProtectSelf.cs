using System;
using Coveo.Bot;
using Coveo.Core;

namespace Coveo.StateMachine
{
    /// <summary>
    /// State where we would locate the nearest tavern to spawn it with healing (So we don't die)
    /// </summary>
    public class ProtectSelf : State
    {
        public override Pos GetGoal(GameState state, TestBot bot)
        {
            throw new NotImplementedException();
        }
    }
}