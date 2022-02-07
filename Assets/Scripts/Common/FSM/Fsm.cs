namespace TankShooter.Common.FSM
{
    public abstract class FsmState<TStateEntity>
    {
        protected readonly TStateEntity Entity;
        
        protected FsmState(TStateEntity entity)
        {
            Entity = entity;
        }

        public virtual void OnEnter() { }

        public virtual FsmState<TStateEntity> Update()
        {
            return this;
        }

        public virtual void OnLeave() { }
    }
    
    public class Fsm<TEntityState>
    {
        private FsmState<TEntityState> state;

        public Fsm(FsmState<TEntityState> state)
        {
            this.state = state;
            state?.OnEnter();
        }

        public void Update()
        {
            if (state != null)
            {
                var newState = state.Update();
                if (ReferenceEquals(state, newState) != true)
                {
                    state.OnLeave();
                    state = newState;
                    state.OnEnter();
                }
            }
        }
    }
}
