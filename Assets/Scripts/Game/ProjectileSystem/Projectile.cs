using System;
using System.Threading;
using TankShooter.Common;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Game.Weapon
{
    /// <summary>
    /// контекст нужен вместо DI, чтобы можно было явно прокинуть какие-то связи на фабрики + какую-то логику,
    /// например ссылку на того, кто выстрелил или цель, куда данный снаряд должен лететь, если он управляем 
    /// </summary>
    public abstract class ProjectileContext<TWeapon> : ActionDisposable
        where TWeapon : class
    {
        //оружие, из которого отправлен данный снаряд
        public readonly TWeapon Weapon;

        public ProjectileContext(TWeapon weapon, Action disposeCallback) : base(disposeCallback)
        {
            this.Weapon = weapon;
        }
    }

    /// <summary>
    /// этот интерфейс нужен, чтобы из любого оружия для танка можно было дернуть метод init
    /// и передать контекст снаряда/пули/ракеты, который содержит колбэк на релиз и
    /// данные для логики боя, например урон, кто стрелял, цель и т.д.
    /// </summary>
    /// <typeparam name="TWeapon">тип оружия, чтобы можно было не заводить новые</typeparam>
    /// <typeparam name="TContext"></typeparam>
    public interface IProjectile<TWeapon, TContext>
        where TWeapon : class
        where TContext : ProjectileContext<TWeapon>
    {
        void Init(TContext context);
    }

    /// <summary>
    /// этот класс снаряда нужен для того, чтобы мы могли навесить его на префаб и иметь
    /// общий префаб в рамках одной системы, а не делать по 3-5 менеджеров или списков внутри
    /// </summary>
    public abstract class Projectile : NotifiableMonoBehaviour
    {
        protected sealed override void SafeAwake()
        {
            base.SafeAwake();
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void UpdateVisual(float dt)
        {
        }

        public virtual void UpdatePhysics(float dt)
        {
        }
    }
    
    /// <summary>
    /// далее абстрактная реализация уже для того, чтобы мы могли передать в определенный тип пули/снаряда или чего-то
    /// еще конкретный контекст, с данными, необходимыми конкретной сущности
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class Projectile<TWeapon, TContext> : Projectile, IProjectile<TWeapon, TContext>
        where TContext : ProjectileContext<TWeapon> where TWeapon : class
    {
        protected TContext context { get; private set; }

        public void Init(TContext context) 
        {
            this.context = context;
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        protected sealed override void OnDisable()
        {
            context?.Dispose();
            context = null;
        }
    }
}