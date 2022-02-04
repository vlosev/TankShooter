using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankShooter.Common.StateMachine
{
    public abstract class FsmState
    {
        public virtual void OnEnter()
        {
        }

        public virtual FsmState Update()
        {
            return this;
        }

        public virtual void OnLeave()
        {
        }
    }
    
    public class Fsm<TState>
    {
        void RegisterState<TState>(TState state)
            where TState : FsmState
        {
        }
    }
}
