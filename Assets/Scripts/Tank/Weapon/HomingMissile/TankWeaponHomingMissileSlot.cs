using UnityEngine;

namespace TankShooter.Tank.Weapon.HommingMissile
{
    /// <summary>
    /// слот для самонаводящихся ракет, они ставятся сюда и отсюда можно получить кол-во слотов
    /// чтобы понимать, сколько именно ракет одновременно может быть на башне конкретного танка
    /// то есть это одновременно настройка и view и logic
    /// </summary>
    public class TankWeaponHomingMissileSlot : TankWeaponSlot
    {
        [SerializeField] private HomingMissileSlot[] missilePivots;

        public HomingMissileSlot[] GetMissilePivots()
        {
            return missilePivots;
        }
    }
}