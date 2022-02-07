using System;
using TankShooter.Common;
using TankShooter.Game.Enemy;
using UnityEngine;
using Common;

namespace TankShooter.Game
{
    public class EnemyEntity : NotifiableMonoBehaviour
    {
        private EnemyAI enemyAi;
        private IHealthEntity healthEntity;
        private Animator animator;
        
        public event Action<EnemyEntity> OnDeadEnemy;

        public EnemyAI AI => enemyAi;

        protected override void SafeAwake()
        {
            base.SafeAwake();
            if (TryGetComponent<IHealthEntity>(out var healthEntity))
            {
                healthEntity.IsAlive.SubscribeChanged(OnIsAliveChanged).SubscribeToDispose(this);
                this.healthEntity = healthEntity;
            }
            
            enemyAi = GetComponent<EnemyAI>();
            if (this.TryGetComponentInChildren(out EnemyTrigger trigger))
                trigger.SetEnemyEntity(this);
        }

        private void OnIsAliveChanged(bool isAlive)
        {
            if (isAlive != true)
            {
                OnDeadEnemy?.Invoke(this);
            }
        }
    }
}