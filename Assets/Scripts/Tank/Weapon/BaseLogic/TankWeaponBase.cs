using System;
using TankShooter.Common;
using TankShooter.Game.Weapon;
using TankShooter.GameInput;
using UnityEngine;

namespace TankShooter.Tank.Weapon
{
    /// <summary>
    /// type:
    /// оружие танка
    /// parameters:
    /// углы ограничения поворота относительно танка
    /// углы ограничения наклона/подъема
    /// время перезарядки
    /// кол-во выстрелов без перезарядки
    /// </summary>
    public abstract class TankWeaponBase : NotifiableMonoBehaviour, IReloadableWeapon
    {
        private IDisposable shootingSubscribe;
        private IDisposable reloadingSubscribe;

        protected ProjectileManager ProjectileManager { get; private set; }
        protected readonly ReactiveProperty<WeaponState> state = new ReactiveProperty<WeaponState>();
        protected readonly ReactiveProperty<float> reloadingProgress = new ReactiveProperty<float>();

        protected TankWeaponManager WeaponManager { get; private set; }
        protected TankWeaponSlot WeaponSlot { get; private set; }

        public abstract TankWeaponSlotName SlotName { get; }
        public IReadonlyReactiveProperty<WeaponState> State => state;
        public IReadonlyReactiveProperty<float> ReloadingProgress => reloadingProgress;
        
        public virtual void Init(TankWeaponManager weaponManager, TankWeaponSlot weaponSlot)
        {
            this.WeaponManager = weaponManager;
            this.WeaponSlot = weaponSlot;
            this.ProjectileManager = weaponManager.Ctx.ProjectileManager;
        }

        public virtual void BindInputController(ITankInputController inputController)
        {
            if (inputController != null)
            {
                shootingSubscribe = inputController.Shooting.SubscribeChanged(OnShootingChanged);
                
                inputController.DoReloadingWeaponEvent += OnReloadingHandle;
                reloadingSubscribe = new ActionDisposable(() => inputController.DoReloadingWeaponEvent -= OnReloadingHandle);
                
                Debug.Log($"Weapon '{GetType().Name}' bind input controller");
            }
            else
            {
                reloadingSubscribe?.Dispose();
                reloadingSubscribe = null;
                
                shootingSubscribe?.Dispose();
                shootingSubscribe = null;
                
                Debug.Log($"Weapon '{GetType().Name}' unbind input controller");
            }
        }

        protected virtual void OnShootingChanged(bool isShooting)
        {
        }

        protected virtual void OnReloadingHandle()
        {
        }
    }
}