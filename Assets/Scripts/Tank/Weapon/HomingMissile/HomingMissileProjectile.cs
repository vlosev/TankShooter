using System;
using TankShooter.Game.Weapon;

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
    }
}