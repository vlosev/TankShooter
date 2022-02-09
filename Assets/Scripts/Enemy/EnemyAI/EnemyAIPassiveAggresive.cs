using TankShooter.Common.FSM;
using UnityEngine;
using UnityEngine.AI;

namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// пассивно агрессивная модель:
    /// эта модель агрится только в случае угрозы, то есть при приближении к врагу или нанесении урона врагом
    /// </summary>
    public class EnemyAIPassiveAggresive : EnemyAI<EnemyAIPassiveAggresive>
    {
        [SerializeField] private float minTimeForFindPath = 0f;
        [SerializeField] private float maxTimeForFindPath = 2f;

        protected override EnemyAIState GetInitialAIState()
        {
            return new StateInit(this);
        }

        #region fsm
        private class StateInit : EnemyAIState
        {
            public StateInit(EnemyAIPassiveAggresive ai) : base(ai)
            {
            }

            public override FsmState<EnemyAIPassiveAggresive> Update()
            {
                return new StateFindDestinationPoint(entity);
            }
        }
        
        private class StateFindDestinationPoint : EnemyAIState
        {
            private float timeToFindPath = 0f;
            
            public StateFindDestinationPoint(EnemyAIPassiveAggresive entity) : base(entity)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
                timeToFindPath = Random.Range(entity.minTimeForFindPath, entity.maxTimeForFindPath);
            }

            public override FsmState<EnemyAIPassiveAggresive> Update()
            {
                timeToFindPath -= entity.DeltaTime;
                if (timeToFindPath <= 0f)
                {
                    return new StateWalk(entity, entity.GetNextPoint());
                }

                return this;
            }
        }
        
        private class StateWalk : EnemyAIState
        {
            private readonly NavMeshAgent agent;
            private readonly Vector3 destinationPoint;
            
            public StateWalk(EnemyAIPassiveAggresive entity, Vector3 destinationPoint) : base(entity)
            {
                this.agent = entity.Agent;
                this.destinationPoint = destinationPoint;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                agent.SetDestination(destinationPoint);
            }

            public override FsmState<EnemyAIPassiveAggresive> Update()
            {
                //агент почему-то остановился
                //TODO: тут, может быть, какое-то время подумаем, куда идти
                if (agent.velocity.magnitude < 0.15f)
                {
                    return new StateFindDestinationPoint(entity);
                }
                
                return base.Update();
            }
        }
        #endregion
    }
}