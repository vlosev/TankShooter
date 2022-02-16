using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Tank.Weapon
{
    public enum TankWeaponSlotName
    {
        /// <summary>
        /// основная пушка, она должна быть всегда
        /// </summary>
        Gun = 0,
        
        /// <summary>
        /// пулемет, он может быть, а может не быть
        /// </summary>
        MachineGun = 1,

        /// <summary>
        /// самонаводящиеся ракеты, они тоже могут быть, а может и не быть
        /// </summary>
        HomingMissile = 2
    }

    public class TankWeaponSlot : NotifiableMonoBehaviour
    {
        [SerializeField] private TankWeaponSlotName slotName;
        [SerializeField] private Transform slotOverrideParentTransform;

        private ITank tank;
        private TankWeaponBase slotWeapon;

        private TankWeaponManager tankWeaponManager;
        
        public Transform WeaponParentTransform
        {
            get
            {
                if (slotOverrideParentTransform != null)
                    return slotOverrideParentTransform;
                return transform;
            }
        }

        /// <summary>
        /// имя слота, оно нужно, чтобы идентифицировать слот при установке оружия,
        /// и чтобы можно было понимать, занят ли этот слот чем-то
        /// </summary>
        public TankWeaponSlotName SlotName => slotName;

        public TankWeaponBase Weapon => slotWeapon;

        public void Init(TankWeaponManager tankWeaponManager)
        {
            this.tankWeaponManager = tankWeaponManager;

            if (slotWeapon != null)
            {
                slotWeapon.Init(tankWeaponManager, this);
            }
        }

        public void SetWeapon(TankWeaponBase weapon)
        {
            if (slotWeapon != weapon)
            {
                if (slotWeapon != null)
                {
                    OnDetachWeapon(slotWeapon);
                    slotWeapon = null;
                }

                slotWeapon = weapon;
                if (slotWeapon != null)
                {
                    OnAttachWeapon(slotWeapon);
                }
            }
        }

        protected virtual void OnAttachWeapon(TankWeaponBase weaponBase)
        {
            var parent = transform;
            if (slotOverrideParentTransform != null)
            {
                parent = slotOverrideParentTransform;
            }

            var weaponTransform = weaponBase.transform;
            if (weaponTransform != null)
            {
                weaponTransform.SetParent(parent);
                weaponTransform.localPosition = Vector3.zero;
                weaponTransform.localRotation = Quaternion.identity;
            }
        }

        protected virtual void OnDetachWeapon(TankWeaponBase weaponBase)
        {
        }
    }
}