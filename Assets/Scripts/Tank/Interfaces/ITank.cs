using TankShooter.GameInput;
using UnityEngine;

namespace Tank.Interfaces
{
    /// <summary>
    /// интерфейс танка необходим для того, чтобы остальные дочерние модули могли с ним взаимодействовать
    /// </summary>
    public interface ITank
    {
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