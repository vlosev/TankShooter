using TankShooter.GameInput;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Tank
{
    public class TankContext
    {
        public readonly TankWeaponManagerContext WeaponManagerCtx;
        
        public TankContext(TankWeaponManagerContext weaponManagerCtx)
        {
            WeaponManagerCtx = weaponManagerCtx;
        }
    }

    /// <summary>
    /// интерфейс танка необходим для того, чтобы остальные дочерние модули могли с ним взаимодействовать
    /// </summary>
    public interface ITank
    {
        TankContext Context { get; }
        Transform Transform { get; }
        Rigidbody Rigidbody { get; }
        ITankInputController InputController { get; }
    }
    
    /// <summary>
    /// интерфейс модуля танка необходим для того, чтобы танк мог инициализировать все необходимое нужными данными
    /// </summary>
    public interface ITankModule
    {
        void Init(ITank tank);
    }
}