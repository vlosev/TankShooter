using UnityEngine;

namespace TankShooter.Battle
{
    public abstract class TankAutomaticWeaponBase : TankWeaponBase
    {
        protected bool doShot = false;

        protected override void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                Debug.Log($"Do automation weapon '{GetType().Name}' shot!");
            }
        }
    }
}