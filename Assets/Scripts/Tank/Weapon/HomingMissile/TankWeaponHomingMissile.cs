using System;
using System.Collections.Generic;
using System.Linq;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Weapon.HommingMissile
{
    /// <summary>
    /// самонаводящиеся ракеты
    /// </summary>
    public class TankWeaponHomingMissile : TankSingleShotWeaponBase
    {
        private class MissileSlotState
        {
            public readonly HomingMissileSlot Slot;
            public bool IsAvailableShot { get; set; }

            public MissileSlotState(HomingMissileSlot slot)
            {
                Slot = slot;
                IsAvailableShot = true;
            }
        }
        
        [SerializeField] private HomingMissileProjectile missilePrefab = null;

        //сколько ракет сейччас нам доступно для выстрелов
        private int missilesCount;

        //слоты для ракет, которые на башне, они и определяют, сколько ракет может нести танк
        private MissileSlotState[] states;
        
        //здесь лежит столько ракет, сколько есть слотов у танка, чтобы потом не инстансить префабы, а просто включать/выключать
        private HomingMissileProjectile[] missiles;

        public override TankWeaponSlotName SlotName => TankWeaponSlotName.HomingMissile;

        public override void Init(TankWeaponManager weaponManager, TankWeaponSlot weaponSlot)
        {
            base.Init(weaponManager, weaponSlot);
            if (weaponSlot is TankWeaponHomingMissileSlot missileSlot)
            {
                states = missileSlot.GetMissilePivots().Select(slot => new MissileSlotState(slot)).ToArray();
            }

            missiles = new HomingMissileProjectile[states.Length];
            for (int i = 0; i < states.Length; ++i)
            {
                var slot = states[i].Slot;
                var missile = Instantiate(missilePrefab);
                var missileTransform = missile.transform;
                missileTransform.SetParent(slot.MissilePivot);
                missileTransform.localPosition = Vector3.zero;
                missileTransform.localRotation = Quaternion.identity;

                missiles[i] = missile;
            }

            ReloadSlots(missiles.Length, true);
        }

        private void Update()
        {
            if (isShot)
            {
                isShot = false;
                
                //TODO: находим первый по счету заряженный слот и стартуем оттуда ракету
                if (TryGetFirstAvailableSlot(out var state, out var index))
                {
                    var projectileManager = WeaponManager.Ctx.ProjectileManager;
                    var projectile = projectileManager.GetProjectile(missilePrefab);
                    
                    if (projectile is IProjectile<TankWeaponHomingMissile, HomingMissileProjectileContext> contextHolder)
                    {
                        var ctx = new HomingMissileProjectileContext(this, () => projectileManager.ReleaseProjectile(projectile));
                        contextHolder.Init(ctx);

                        //1) спавним вторую ракету, которая должна быть
                        projectile.transform.position = state.Slot.MissilePivot.position;
                        projectile.transform.forward = state.Slot.MissilePivot.forward;

                        //2) говорим, что слот разряжен, чтобы при следующем выстереле взять другой
                        state.IsAvailableShot = false;
                        
                        //2) скрываем ракету, которая находится в слоте
                        missiles[index].gameObject.SetActive(false);
                    }
                }
            }
        }

        private bool TryGetFirstAvailableSlot(out MissileSlotState result, out int slotNumber)
        {
            for (int i = 0; i < states.Length; ++i)
            {
                var state = states[i];
                if (state.IsAvailableShot)
                {
                    slotNumber = i;
                    result = state;
                    return true;
                }
            }

            slotNumber = -1;
            result = null;
            return false;
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