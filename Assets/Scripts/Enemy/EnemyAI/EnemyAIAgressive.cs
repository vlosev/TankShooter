using TankShooter.Common.FSM;
using UnityEngine;

namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// агрессивная модель:
    /// эта модель знает врага в лицо, целенаправленно идет к нему и пытается нанести урон
    /// </summary>
    public class EnemyAIAgressive : EnemyAI
    {
        protected override EnemyAIState GetInitialAIState()
        {
            return new StateInit(this);
        }
        
        protected class StateInit : EnemyAIState
        {
            public StateInit(EnemyAI ai) : base(ai)
            {
            }

            public override FsmState<EnemyAI> Update()
            {
                return new StateAgro(Entity);
            }
        }
        
        protected class StateAgro : EnemyAIState
        {
            public StateAgro(EnemyAI ai) : base(ai)
            {
            }
        }
    }
}