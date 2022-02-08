using System;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.GameInput
{
    public abstract class InputController : NotifiableMonoBehaviour, ITankInputController, ICameraInputController
    {
        protected readonly ReactiveProperty<float> acceleration = new ReactiveProperty<float>();
        protected readonly ReactiveProperty<float> steering = new ReactiveProperty<float>();
        protected readonly ReactiveProperty<bool> shooting = new ReactiveProperty<bool>();
        protected readonly ReactiveProperty<Vector3> targetPoint = new ReactiveProperty<Vector3>();
        
        protected readonly ReactiveProperty<bool> cameraMove = new ReactiveProperty<bool>();
        protected readonly ReactiveProperty<float> cameraZoomDelta = new ReactiveProperty<float>();
        protected readonly ReactiveProperty<Vector2> cameraMoveDelta = new ReactiveProperty<Vector2>();

        //tank input
        IReadonlyReactiveProperty<float> ITankInputController.Acceleration => acceleration;
        IReadonlyReactiveProperty<float> ITankInputController.Steering => steering;
        IReadonlyReactiveProperty<Vector3> ITankInputController.TargetPoint => targetPoint;
        IReadonlyReactiveProperty<bool> ITankInputController.Shooting => shooting;
        
        //camera input
        IReadonlyReactiveProperty<float> ICameraInputController.CameraZoomDelta => cameraZoomDelta; 
        IReadonlyReactiveProperty<bool> ICameraInputController.CameraMove => cameraMove;
        IReadonlyReactiveProperty<Vector2> ICameraInputController.CameraMoveDelta => cameraMoveDelta;
        
        public event Action DoReloadingWeaponEvent;
        public event Action DoSelectPrevWeaponEvent;
        public event Action DoSelectNextWeaponEvent;
        public event Action<int> DoSelectWeaponEvent;

        protected void DoReloadingWeapon()
        {
            DoReloadingWeaponEvent?.Invoke();
        }

        protected void DoSelectPrevWeapon()
        {
            DoSelectPrevWeaponEvent?.Invoke();
        }

        protected void DoSelectNextWeapon()
        {
            DoSelectNextWeaponEvent?.Invoke();
        }

        protected void DoSelectWeaponByNumber(int number)
        {
            DoSelectWeaponEvent?.Invoke(number);
        }
    }
}