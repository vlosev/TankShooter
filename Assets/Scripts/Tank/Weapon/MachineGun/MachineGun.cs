using TankShooter.Battle;
using TankShooter.Battle.TankCode;

namespace Tank.Weapon.MachineGun
{
    public class MachineGun : TankAutomaticWeaponBase
    {
        public override TankWeaponSlotName SlotName => TankWeaponSlotName.MachineGun;
    }
}