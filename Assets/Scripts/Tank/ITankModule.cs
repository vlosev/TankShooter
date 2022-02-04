using UnityEngine;

namespace TankShooter.Battle
{
    public interface ITank
    {
        IInputController InputContoller { get; }

        Rigidbody Rigidbody { get; }
    }
    
    public interface ITankModule
    {
        void SetupModule(ITank tank);
    }
}