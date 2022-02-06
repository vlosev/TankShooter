using TankShooter.Battle;
using UnityEngine;

namespace Tank.Interfaces
{
    /// <summary>
    /// интерфейс танка необходим для того, чтобы остальные дочерние модули могли с ним взаимодействовать
    /// </summary>
    public interface ITank
    {
        Rigidbody Rigidbody { get; }
        ITankInputController InputController { get; }
    }
}