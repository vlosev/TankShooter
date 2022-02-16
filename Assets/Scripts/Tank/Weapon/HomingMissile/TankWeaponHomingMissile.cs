using System.Linq;
using UnityEngine;

namespace TankShooter.Tank.Weapon.HommingMissile
{
    /// <summary>
    /// самонаводящиеся ракеты
    /// </summary>
    public class TankWeaponHomingMissile : TankSingleShotWeaponBase
    {
        [SerializeField] private HomingMissileProjectile missilePrefab = null;

        //сколько ракет сейччас нам доступно для выстрелов
        private int missilesCount;

        //слоты для ракет, которые на башне, они и определяют, сколько ракет может нести танк
        private HomingMissileSlot[] slots;
        
        //здесь лежит столько ракет, сколько есть слотов у танка, чтобы потом не инстансить префабы, а просто включать/выключать
        private HomingMissileProjectile[] missiles;

        public override TankWeaponSlotName SlotName => TankWeaponSlotName.HomingMissile;

        public override void Init(TankWeaponManager weaponManager, TankWeaponSlot weaponSlot)
        {
            base.Init(weaponManager, weaponSlot);
            if (weaponSlot is TankWeaponHomingMissileSlot missileSlot)
                slots = missileSlot.GetMissilePivots().ToArray();

            missiles = new HomingMissileProjectile[slots.Length];
            for (int i = 0; i < slots.Length; ++i)
            {
                var slot = slots[i];
                var missile = Instantiate(missilePrefab);
                var missileTransform = missile.transform;
                missileTransform.SetParent(slot.MissilePivot);
                missileTransform.localPosition = Vector3.zero;
                missileTransform.localRotation = Quaternion.identity;

                missiles[i] = missile;
            }

            ReloadSlots(missiles.Length, true);
        }

        private void ReloadSlots(int slotsCount, bool force = false)
        {
            for (int i = 0; i < slotsCount; ++i)
                ReloadSlots(i, force);
        }

        private void ReloadSlot(int slot, bool force = false)
        {
            //TODO: тут может быть будет какая-то анимация
            missiles[slot].gameObject.SetActive(true);
        }
    }
}