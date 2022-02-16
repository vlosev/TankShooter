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
        
        private float lostTime;

        protected override void OnInit()
        {
            base.OnInit();
            
            lostTime = lifeTime;
        }

        public override void UpdateVisual(float dt)
        {
            if (lostTime <= 0f)
            {
                gameObject.SetActive(false);
            }
            
            lostTime -= dt;
        }
    }
}