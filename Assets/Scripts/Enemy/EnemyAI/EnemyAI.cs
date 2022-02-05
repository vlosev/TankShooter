using System;
using System.Collections;
using System.Collections.Generic;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Game.Enemy
{
    public abstract class EnemyAI : NotifiableMonoBehaviour
    {
        protected abstract class EnemyAIState
        {
            protected readonly EnemyAI AI;
            
            protected EnemyAIState(EnemyAI ai)
            {
                this.AI = ai;
            }

            public virtual void OnEnter() { }

            public virtual EnemyAIState OnTick(float dt)
            {
                return this;
            }
            
            public virtual void OnLeave() { }
        }

        private EnemyAIState state;
        
        protected override void SafeAwake()
        {
            base.SafeAwake();
            state = GetInitialAIState();
        }

        private void Update()
        {
            if (state != null)
            {
                var newState = state.OnTick(Time.deltaTime);
                if (ReferenceEquals(state, newState) != true)
                {
                    state.OnLeave();
                    state = newState;
                    state.OnEnter();
                }
            }
        }
        
        //создает стартовый стейт врага и потом дальше стейт машина переключает стейты,
        //получая следующей стейт из текущего
        protected abstract EnemyAIState GetInitialAIState();
    }
}