using System.Runtime.CompilerServices;
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
    public abstract class TankWeapon : NotifiableMonoBehaviour, IReloadableWeapon
    {
        protected readonly ReactiveProperty<WeaponState> state = new ReactiveProperty<WeaponState>();
        protected readonly ReactiveProperty<float> reloadingProgress = new ReactiveProperty<float>();

        public IReadonlyReactiveProperty<WeaponState> State => state;

        public IReadonlyReactiveProperty<float> ReloadingProgress => reloadingProgress;

        public abstract void Init(ITank tank);
        
        protected override void SafeAwake()
        {
            base.SafeAwake();
            
            state.SubscribeChanged(state =>
            {
                Debug.Log($"Weapon '{GetType().Name}' change state to: '{state}'");
            }).SubscribeToDispose(this);
        }
    }
}