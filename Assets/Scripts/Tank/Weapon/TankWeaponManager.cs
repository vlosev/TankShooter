using System;
using System.Collections.Generic;
using Common;
using Tank.Interfaces;
using TankShooter.Battle;
using TankShooter.Battle.TankCode;
using TankShooter.Common;
using TankShooter.GameInput;
using UnityEngine;

namespace Tank.Weapon
{
    //этот класс сделан монобехейвором только потому что удобнее настраивать в инспекторе
    //логики самого монобехейвора он в себе не несет
    public class TankWeaponManager : MonoBehaviour, ITankModule, ITankInputControllerHandler
    {
        private readonly List<TankWeaponSlot> slots = new List<TankWeaponSlot>();
        
        private int selectedSlotIndex;
        private ITankInputController tankInputController;
        private ReactiveProperty<TankWeaponSlot> selectedSlot = new ReactiveProperty<TankWeaponSlot>();

        public IReadonlyReactiveProperty<TankWeaponSlot> SelectedSlot => selectedSlot;

        public void Init(ITank tank)
        {
            if (tank.Transform.TryGetComponentsInChildren<TankWeaponSlot>(out var foundSlots))
            {
                foreach (var slot in foundSlots)
                {
                    slot.Init(this);
                    slots.Add(slot);
                }
            }
            
            //SetWeapon(0);
        }

        public void AddWeapon(TankWeaponBase weaponBase)
        {
        }
        
        public void BindInputController(ITankInputController inputController)
        {
            //отписываемся сразу на всякий случай
            if (tankInputController != null)
            {
                UnbindInputFromWeapon();
                tankInputController.DoSelectNextWeaponEvent -= SelectNextWeapon;
                tankInputController.DoSelectPrevWeaponEvent -= SelectPrevWeapon;
                tankInputController.DoSelectWeaponEvent -= SelectWeapon;
                tankInputController = null;
            }
            
            //подписываемся/переподписываемся
            tankInputController = inputController;
            if (tankInputController != null)
            {
                tankInputController.DoSelectNextWeaponEvent += SelectNextWeapon;
                tankInputController.DoSelectPrevWeaponEvent += SelectPrevWeapon;
                tankInputController.DoSelectWeaponEvent += SelectWeapon;
                BindInputToWeapon();
            }
        }
        
        private void SelectNextWeapon()
        {
            var index = (selectedSlotIndex + 1) % slots.Count;
            SetWeapon(index);
        }

        private void SelectPrevWeapon()
        {
            var index = (selectedSlotIndex - 1) % slots.Count;
            if (index < 0)
                index = slots.Count + index;
            SetWeapon(index);
        }

        private void SelectWeapon(int index)
        {
            SetWeapon(Math.Max(0, Math.Min(index, slots.Count - 1)));
        }

        private void SetWeapon(int index)
        {
            selectedSlotIndex = index;
            
            var slot = slots[selectedSlotIndex];
            if (selectedSlot.Value != slot)
            {
                UnbindInputFromWeapon();
                //sele = slot;
                BindInputToWeapon();
            }
        }

        private void UnbindInputFromWeapon()
        {
            // if (currentWeaponBase != null)
            //     currentWeaponBase.BindInputController(null);
        }

        private void BindInputToWeapon()
        {
            // if (currentWeaponBase != null)
            //     currentWeaponBase.BindInputController(tankInputController);
        }
    }
}