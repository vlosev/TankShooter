using UnityEngine;

namespace TankShooter.Battle.TankCode
{
    /// <summary>
    /// слот для самонаводящихся ракет, они ставятся сюда и отсюда могут получить
    /// сколько именно ракет одновременно может быть на башне конкретного танка 
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