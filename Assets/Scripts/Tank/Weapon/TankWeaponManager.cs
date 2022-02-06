using System;
using Tank.Interfaces;
using TankShooter.Battle;
using UnityEngine;

namespace Tank.Weapon
{
    //этот класс сделан монобехейвором только потому что удобнее настраивать в инспекторе
    //логики самого монобехейвора он в себе не несет
    public class TankWeaponManager : MonoBehaviour, ITankModule, ITankInputControllerHandler
    {
        [SerializeField] private TankWeapon[] weapons;

        private int currentWeaponIndex;
        private ITankInputController tankInputController;
        private TankWeapon currentWeapon;

        public void Init(ITank tank)
        {
            SetWeapon(0);
        }

        public void BindInputController(ITankInputController tankInputController)
        {
            this.tankInputController = tankInputController;
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
            var index = (currentWeaponIndex + 1) % weapons.Length;
            SetWeapon(index);
        }

        private void SelectPrevWeapon()
        {
            var index = (currentWeaponIndex - 1) % weapons.Length;
            if (index < 0)
                index = weapons.Length + index;
            SetWeapon(index);
        }

        private void SelectWeapon(int index)
        {
            SetWeapon(Math.Max(0, Math.Min(index, weapons.Length - 1)));
        }

        private void SetWeapon(int index)
        {
            currentWeaponIndex = index;
            
            var weapon = weapons[currentWeaponIndex];
            if (currentWeapon != weapon)
            {
                UnbindInputFromWeapon();
                currentWeapon = weapon;
                BindInputToWeapon();
            }
        }

        private void UnbindInputFromWeapon()
        {
            if (currentWeapon != null)
            {
                currentWeapon.BindInputController(null);
            }
        }

        private void BindInputToWeapon()
        {
            if (currentWeapon != null)
            {
                currentWeapon.BindInputController(tankInputController);
            }
        }
    }
}