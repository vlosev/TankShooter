using UnityEngine;

namespace TankShooter.Battle
{
    public class TankAutomaticWeapon : TankWeapon
    {
        protected bool doShot = false;
        
        public override void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                Debug.Log($"Do automation weapon '{GetType().Name}' shot!");
            }
        }

    }
}