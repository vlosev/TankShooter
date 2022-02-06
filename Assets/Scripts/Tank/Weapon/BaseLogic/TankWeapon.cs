using System;
using Tank.Weapon;
using TankShooter.Common;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Battle
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
    public abstract class TankWeapon : NotifiableMonoBehaviour, IReloadableWeapon, ITankInputControllerHandler
    {
        private IDisposable shootingSubscribe;
        private IDisposable reloadingSubscribe;

        protected readonly ReactiveProperty<WeaponState> state = new ReactiveProperty<WeaponState>();
        protected readonly ReactiveProperty<float> reloadingProgress = new ReactiveProperty<float>();

        protected TankWeaponManager tankWeaponManager { get; private set; }

        public IReadonlyReactiveProperty<WeaponState> State => state;
        public IReadonlyReactiveProperty<float> ReloadingProgress => reloadingProgress;
        
        protected override void SafeAwake()
        {
            base.SafeAwake();
            
            state.SubscribeChanged(state =>
            {
                Debug.Log($"Weapon '{GetType().Name}' change state to: '{state}'");
            }).SubscribeToDispose(this);
        }

        public virtual void Init(TankWeaponManager tankWeaponManager)
        {
            this.tankWeaponManager = tankWeaponManager;
        }

        public virtual void BindInputController(ITankInputController inputController)
        {
            if (inputController != null)
            {
                shootingSubscribe = inputController.Shooting.SubscribeChanged(OnShootingChanged);
                
                inputController.DoReloadingWeaponEvent += OnReloadingHandle;
                reloadingSubscribe = new ActionDisposable(() => inputController.DoReloadingWeaponEvent -= OnReloadingHandle);
            }
            else
            {
                reloadingSubscribe?.Dispose();
                reloadingSubscribe = null;
                
                shootingSubscribe?.Dispose();
                shootingSubscribe = null;
            }
        }
        
        public virtual void OnShootingChanged(bool isShooting)
        {
            throw new NotImplementedException();
        }    

        public virtual void OnReloadingHandle()
        {
            throw new NotImplementedException();
        }
    }
}