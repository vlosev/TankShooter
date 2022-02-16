using System;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Weapon.MachineGun
{
    public class MachineGunBulletContext : ProjectileContext<TankWeaponMachineGun>
    {
        public MachineGunBulletContext(TankWeaponMachineGun weapon, Action disposableCallback) 
            : base(weapon, disposableCallback)
        {
        }
    }
    
    public class MachineGunBullet : Projectile<TankWeaponMachineGun, MachineGunBulletContext>
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

        public override void UpdatePhysics(float dt)
        {
        }
    }
}