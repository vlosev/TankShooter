using Common;
using TankShooter.Common.FSM;
using TankShooter.Tank;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// агрессивная модель:
    /// эта модель знает врага в лицо, целенаправленно идет к нему и пытается нанести урон
    /// </summary>
    public class EnemyAIAgressive : EnemyAI<EnemyAIAgressive>
    {
        [SerializeField] private float minTimeForFindEnemy = 0f;
        [SerializeField] private float maxTimeForFindEnemy = 2f;

        protected override EnemyAIState GetInitialAIState()
        {
            return new StateInit(this);
        }

        #region fsm
        private class StateInit : EnemyAIState
        {
            public StateInit(EnemyAIAgressive ai) : base(ai)
            {
            }

            public override FsmState<EnemyAIAgressive> Update()
            {
                return new StateWalk(entity);
            }
        }
        
        //этот тип врага каждый Н секунд ищет своего врага, то есть игрока
        private class StateWalk : EnemyAIState
        {
            private readonly NavMeshAgent agent;
            private float timeForFindEnemy;
            
            public StateWalk(EnemyAIAgressive entity) : base(entity)
            {
                this.agent = entity.Agent;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                FindEnemyPosition();
            }

            public override FsmState<EnemyAIAgressive> Update()
            {
                if (timeForFindEnemy <= 0f)
                {
                    FindEnemyPosition();
                }

                timeForFindEnemy -= entity.DeltaTime;
                return base.Update();
            }

            private void FindEnemyPosition()
            {
                //TODO: пробросить через контекст чтобы знать на кого агриться
                var tank = FindObjectOfType<TankController>();
                if (tank != null)
                {
                    if (tank.TryGetComponentInChildren<BoxCollider>(out var tankBox))
                    {
                        var closestPoint = tankBox.ClosestPoint(entity.transform.position);
                        agent.SetDestination(closestPoint);
                    }
                }
                
                timeForFindEnemy = Random.Range(entity.minTimeForFindEnemy, entity.maxTimeForFindEnemy);
            }
        }
        #endregion
    }
}