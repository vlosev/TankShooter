using TankShooter.Battle;
using TankShooter.Battle.TankCode;

namespace Tank.Weapon.HomingMissile
{
    /// <summary>
    /// самонаводящиеся ракеты
    /// </summary>
    public class TankWeaponBaseHomingMissile : TankSingleShotWeaponBase
    {
        public override TankWeaponSlotName SlotName => TankWeaponSlotName.HomingMissile;
    }
}