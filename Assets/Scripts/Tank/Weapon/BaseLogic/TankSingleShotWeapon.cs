using Tank.Interfaces;
using UnityEngine;

namespace TankShooter.Battle
{
    public class TankSingleShotWeapon : TankWeapon
    {
        protected bool isShot = false;
        // public bool IsShot
        // {
        //     get
        //     {
        //         if (isShot)
        //         {
        //             isShot = false;
        //             return true;
        //         }
        //
        //         return false;
        //     }
        // }
        
        public void Init(ITank tank)
        {
            //do nothing
        }

        public override void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                isShot = true;
            }
        }
    }
}