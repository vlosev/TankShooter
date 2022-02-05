using System;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Game
{
    public class EnemyEntity : NotifiableMonoBehaviour
    {
        private IHealthEntity healthEntity;
        private Animator animator;

        public event Action<EnemyEntity> OnDeadEnemy;

        protected override void SafeAwake()
        {
            base.SafeAwake();

            if (TryGetComponent<IHealthEntity>(out var healthEntity))
            {
                healthEntity.IsAlive.SubscribeChanged(OnIsAliveChanged).SubscribeToDispose(this);

                this.healthEntity = healthEntity;
            }
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