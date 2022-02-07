using System;
using System.Collections;
using System.Collections.Generic;
using TankShooter.Common;
using TankShooter.Common.FSM;
using UnityEngine;
using UnityEngine.AI;

namespace TankShooter.Game.Enemy
{
    public abstract class EnemyAI : NotifiableMonoBehaviour
    {
        protected class EnemyAIState : FsmState<EnemyAI>
        {
            public EnemyAIState(EnemyAI entity) : base(entity)
            {
            }
        }

        [SerializeField] private NavMeshAgent agent;

        private Fsm<EnemyAI> fsm;

        public float DeltaTime => Time.deltaTime;
        public NavMeshAgent Agent => agent;

        public void StartEnemy()
        {
            fsm = new Fsm<EnemyAI>(GetInitialAIState());
        }

        private void Update()
        {
            fsm.Update();
        }

        //создает стартовый стейт врага и потом дальше стейт машина переключает стейты,
        //получая следующей стейт из текущего
        protected abstract EnemyAIState GetInitialAIState();
    }
}