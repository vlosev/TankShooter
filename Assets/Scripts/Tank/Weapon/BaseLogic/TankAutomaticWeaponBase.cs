using UnityEngine;

namespace TankShooter.Tank.Weapon
{
    public abstract class TankAutomaticWeaponBase : TankWeaponBase
    {
        protected bool isShooting = false;

        protected override void OnShootingChanged(bool isShooting)
        {
            this.isShooting = isShooting;
        }
    }
}