using System;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Weapon.HommingMissile
{
    public class HomingMissileProjectileContext : ProjectileContext<TankWeaponHomingMissile>
    {
        public HomingMissileProjectileContext(TankWeaponHomingMissile weapon, Action disposeCallback) 
            : base(weapon, disposeCallback)
        {
        }
    }
    
    public class HomingMissileProjectile : Projectile<TankWeaponHomingMissile, HomingMissileProjectileContext>
    {
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private float speed = 10f;

        private float lostTime;

        protected override void OnInit()
        {
            base.OnInit();
            lostTime = lifeTime;
        }

        public override void UpdateVisual(float dt)
        {
            base.UpdateVisual(dt);

            //TODO: compute following trajectory
            
            transform.Translate(transform.forward * speed * dt, Space.World);
            
            if (lostTime <= 0f)
            {
                gameObject.SetActive(false);
            }

            lostTime -= dt;
        }
    }
}