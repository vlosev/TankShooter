using Tank.Interfaces;
using UnityEngine;

namespace TankShooter.Battle
{
    public class TankSingleShotWeapon : TankWeapon
    {
        public void Init(ITank tank)
        {
            //do nothing
        }

        public override void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                Debug.Log($"Do singleshot weapon '{GetType().Name}' shot!");
            }
        }
    }
}