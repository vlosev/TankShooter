using System;
using TankShooter.Game.Weapon;

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
        public override void UpdateVisual(float dt)
        {
        }

        public override void UpdatePhysics(float dt)
        {
        }
    }
}