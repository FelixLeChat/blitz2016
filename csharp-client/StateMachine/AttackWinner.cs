using Coveo.Bot;
using Coveo.Core;

namespace Coveo.StateMachine
{
    public class AttackWinner : State
    {
        public override State CalculateNextState(GameState state, TestBot bot)
        {
            var enemy = GetEnemy(state);

            if (enemy.MineCount <= state.MyHero.MineCount)
            {
                if(state.MyHero.Life > 25)
                    return new CaptureMine();
                return new GoHeal();
            }
            else if(enemy.Life > state.MyHero.Life)
                return new GoHeal();

            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            var enemy = GetEnemy(state);
            return enemy.Pos;
        }

        public Hero GetEnemy(GameState state)
        {
            var enemyPlayer = new Hero();
            var maxMine = 0;

            foreach (var hero in state.Heroes)
            {
                if (hero.MineCount > maxMine)
                {
                    if (hero.Pos != state.MyHero.Pos)
                    {
                        maxMine = hero.MineCount;
                        enemyPlayer = hero;
                    }
                }
            }

            return enemyPlayer;
        }
    }
}