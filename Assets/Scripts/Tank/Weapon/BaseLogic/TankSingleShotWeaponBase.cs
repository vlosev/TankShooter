namespace TankShooter.Tank.Weapon
{
    public abstract class TankSingleShotWeaponBase : TankWeaponBase
    {
        protected bool isShot = false;
        
        protected override void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                isShot = true;
            }
        }
    }
}