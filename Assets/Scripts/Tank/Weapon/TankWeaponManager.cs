using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using TankShooter.Common;
using TankShooter.Game.Weapon;
using TankShooter.GameInput;
using UnityEngine;

namespace TankShooter.Tank.Weapon
{
    public class TankWeaponManagerContext
    {
        public readonly ProjectileManager ProjectileManager;

        public TankWeaponManagerContext(ProjectileManager projectileManager)
        {
            ProjectileManager = projectileManager;
        }
    }
    
    //этот класс сделан монобехейвором только потому что удобнее настраивать в инспекторе
    //логики самого монобехейвора он в себе не несет
    //так же менеджер отдает наружу свойства для управления оружием
    public class TankWeaponManager : MonoBehaviour, ITankModule, ITankInputControllerHandler
    {
        private readonly List<TankWeaponSlot> slots = new List<TankWeaponSlot>();

        private TankWeaponManagerContext ctx;
        private TankWeaponSlot selectedWeaponSlot;
        private int selectedSlotIndex;
        private ITankInputController inputController;

        public TankWeaponManagerContext Ctx => ctx;
        public ITankInputController InputController => inputController;

        public void Init(ITank tank)
        {
            ctx = tank.Context.WeaponManagerCtx;
            
            if (tank.Transform.TryGetComponentsInChildren<TankWeaponSlot>(out var foundSlots))
            {
                slots.AddRange(foundSlots.OrderBy(slot => (int)slot.SlotName));
                slots.ForEach(slot => slot.Init(this));
            }
            
            SetWeapon(0);
        }
        
        public void BindInputController(ITankInputController inputController)
        {
            //отписываемся сразу на всякий случай
            if (this.inputController != null)
            {
                this.inputController.DoSelectNextWeaponEvent -= SelectNextWeapon;
                this.inputController.DoSelectPrevWeaponEvent -= SelectPrevWeapon;
                this.inputController.DoSelectWeaponEvent -= SelectWeapon;
                this.inputController = null;
            }
            
            //подписываемся/переподписываемся
            this.inputController = inputController;
            if (this.inputController != null)
            {
                this.inputController.DoSelectNextWeaponEvent += SelectNextWeapon;
                this.inputController.DoSelectPrevWeaponEvent += SelectPrevWeapon;
                this.inputController.DoSelectWeaponEvent += SelectWeapon;
            }
            
            UpdateInputBinding();
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

            var selectSlot = slots[selectedSlotIndex];
            if (selectSlot != selectedWeaponSlot)
            {
                selectedWeaponSlot = selectSlot;
                UpdateInputBinding();
            }
        }

        private void UpdateInputBinding()
        {
            foreach (var slot in slots)
            {
                var controller = slot == selectedWeaponSlot
                    ? inputController
                    : null;

                var weapon = slot.Weapon;
                if (weapon != null)
                {
                    weapon.BindInputController(controller);
                }
            }
        }
    }
}