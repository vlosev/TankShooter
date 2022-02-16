using System;
using Common;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Weapon.Gun
{
    public class GunProjectileContext : ProjectileContext<TankWeaponGun>
    {
        public GunProjectileContext(TankWeaponGun weapon, Action disposeCallback) 
            : base(weapon, disposeCallback)
        {
        }
    }
    
    public class GunProjectile : Projectile<TankWeaponGun, GunProjectileContext>
    {
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private float speed = 100f;
        
        private float lostTime;

        protected override void OnInit()
        {
            base.OnInit();
            
            lostTime = lifeTime;
        }

        public override void UpdateVisual(float dt)
        {
            transform.Translate(transform.forward * speed * dt, Space.World);
            
            if (lostTime <= 0f)
            {
                gameObject.SetActive(false);
            }
            
            lostTime -= dt;
        }
    }
}