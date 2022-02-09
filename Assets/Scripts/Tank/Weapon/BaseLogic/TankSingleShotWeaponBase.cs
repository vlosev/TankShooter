using Tank.Interfaces;
using UnityEngine;

namespace TankShooter.Battle
{
    public abstract class TankSingleShotWeaponBase : TankWeaponBase
    {
        protected bool isShot = false;
        
        public void Init(ITank tank)
        {
            //do nothing
        }

        protected override void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                isShot = true;
            }
        }
    }
}