using TankShooter.Common;

namespace TankShooter.Game.Weapon
{
    /// <summary>
    /// состояние оружия, глобально их может быть несколько
    /// </summary>
    public enum WeaponState
    {
        //пусто - не готово к стрельбе, например, нет патронов/снарядов
        NotAvailable = 0,
        //оружие в состоянии простоя
        Idle = 1,
        //готово к стрельбе
        Ready = 2,
        //в процессе выстрела
        Shot = 3,
        //в процессе перезарядки
        Reload = 4,
        //сломано
        Crashed = 5,
    }

    /// <summary>
    /// базовое оружие
    /// </summary>
    public interface IWeapon
    {
        IReadonlyReactiveProperty<WeaponState> State { get; }
    }

    /// <summary>
    /// оружие, которое умеет перезаряжаться
    /// </summary>
    public interface IReloadableWeapon : IWeapon
    {
        IReadonlyReactiveProperty<float> ReloadingProgress { get; }
    }
}