using Tank.Interfaces;
using Tank.Weapon;
using TankShooter.Common;
using TankShooter.GameInput;
using UnityEngine;

namespace TankShooter.Battle.TankCode
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

    public class TankWeaponSlot : NotifiableMonoBehaviour, ITankInputControllerHandler
    {
        [SerializeField] private TankWeaponSlotName slotName;

        private ITank tank;

        private TankWeaponManager tankWeaponManager;
        private readonly ReactiveProperty<TankWeaponBase> weapon = new ReactiveProperty<TankWeaponBase>();
        private readonly ReactiveProperty<bool> isActiveSlot = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> isSelectedSlot = new ReactiveProperty<bool>();

        public IReadonlyReactiveProperty<bool> IsActiveSlot => isActiveSlot;
        public IReadonlyReactiveProperty<bool> IsSelectedSlot => isSelectedSlot;
        public IReadonlyReactiveProperty<TankWeaponBase> Weapon => weapon;

        /// <summary>
        /// имя слота, оно нужно, чтобы идентифицировать слот при установке оружия,
        /// и чтобы можно было понимать, занят ли этот слот чем-то
        /// </summary>
        public TankWeaponSlotName SlotName => slotName;

        public void Init(TankWeaponManager tankWeaponManager)
        {
            this.tankWeaponManager = tankWeaponManager;
        }

        public void SetWeapon(TankWeaponBase weapon)
        {
            if (this.weapon.Value == weapon)
                return;

            if (this.weapon.Value != null)
                OnDetachWeapon(this.weapon);
            
            this.weapon.Value = weapon;
            if (this.weapon.Value != null)
                OnAttachWeapon(this.weapon);
        }

        public void SetSelection(bool select)
        {
            isSelectedSlot.Value = select;
        }

        public void BindInputController(ITankInputController inputController)
        {
            // if (tankWeaponBase != null)
            //     tankWeaponBase.BindInputController(inputController);
        }

        protected virtual void OnAttachWeapon(TankWeaponBase weaponBase)
        {
        }

        protected virtual void OnDetachWeapon(TankWeaponBase weaponBase)
        {
        }
    }
}