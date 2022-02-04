using System;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Game
{
    public class HealthEntity : NotifiableMonoBehaviour, 
        IHealthEntity, 
        IDamageReceiver
    {
        [Tooltip("Максимальное здоровье")]
        [SerializeField] private float maxHealth = 100f;

        [Tooltip("Броня в процентах [0; 1], health=health-damage*armor")]
        [SerializeField] private float armor = 0.5f;

        private readonly ReactiveProperty<float> health = new ReactiveProperty<float>();
        private readonly ReactiveProperty<bool> isAlive = new ReactiveProperty<bool>();

        public IReadonlyReactiveProperty<float> Health => health;
        public IReadonlyReactiveProperty<bool> IsAlive => isAlive;

        protected override void SafeAwake()
        {
            base.SafeAwake();
            
            health.SubscribeChanged(OnHealthChanged).SubscribeToDispose(this);
        }

        private void OnHealthChanged(float health)
        {
            if (health == 0f)
            {
                isAlive.Value = false;
            }
        }

        private void Awake()
        {
            armor = Mathf.Clamp01(armor);
            
            health.Value = maxHealth;
            isAlive.Value = true;
        }

        public void OnDamage(float damage)
        {
            if (isAlive.Value)
            {
                health.Value = Mathf.Max(0, health.Value - damage * armor);
            }
        }
    }
}