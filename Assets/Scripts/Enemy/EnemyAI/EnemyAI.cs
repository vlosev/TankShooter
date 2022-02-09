using System;
using System.Diagnostics;
using TankShooter.Common;
using TankShooter.Common.FSM;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

namespace TankShooter.Game.Enemy
{
    public abstract class EnemyAI<TImplEnemyAI> : NotifiableMonoBehaviour
    {
        protected class EnemyAIState : FsmState<TImplEnemyAI>
        {
            public EnemyAIState(TImplEnemyAI entity) : base(entity)
            {
                PrintStateName();
            }

            [Conditional("UNITY_EDITOR")]
            private void PrintStateName()
            {
                Debug.Log($"Entity state '{GetType().Name}' construct!");
            }
        }

        [SerializeField] private EnemyEntity enemyEntity;
        [SerializeField] private NavMeshAgent agent;

        private Fsm<TImplEnemyAI> fsm;

        protected LevelContext levelContext { get; private set; }
        protected EnemyPathManager enemyPathManager { get; private set; }

        public EnemyEntity Entity => enemyEntity;
        public float DeltaTime => Time.deltaTime;
        public NavMeshAgent Agent => agent;

        private void OnEnable()
        {
            enemyEntity.OnStartEnemy += OnStartEnemy;
            enemyEntity.OnDeadEnemy += OnDeadEnemy;
        }
        
        private void Update()
        {
            fsm?.Update();
        }

        private void OnDisable()
        {
            enemyEntity.OnDeadEnemy -= OnDeadEnemy;
            enemyEntity.OnStartEnemy -= OnStartEnemy;
        }

        //фабрика для создания базового стейта инициализации поведения
        protected abstract EnemyAIState GetInitialAIState();

        protected Vector3 GetNextPoint()
        {
            return Entity.Context.PathManager.GetNextPoint(transform.position);
        }
        
        private void OnStartEnemy(EnemyEntity obj)
        {
            fsm = new Fsm<TImplEnemyAI>(GetInitialAIState());

            Debug.Log($"On start enemy with model ai '{this.GetType().Name}'");
        }

        private void OnDeadEnemy(EnemyEntity obj)
        {
            fsm = null;
            
            Debug.Log($"On dead enemy with model ai '{this.GetType().Name}'");
        }
    }
}