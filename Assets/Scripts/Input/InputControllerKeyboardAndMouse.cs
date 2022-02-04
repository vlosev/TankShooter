using System;
using TankShooter.Common;
using UnityEngine;
using UnityEngineInternal;

namespace TankShooter.Battle
{
    public class InputControllerKeyboardAndMouse : NotifiableMonoBehaviour, IInputController
    {
        [SerializeField] private Camera playerCamera;

        private readonly ReactiveProperty<float> acceleration = new ReactiveProperty<float>();
        private readonly ReactiveProperty<float> steering = new ReactiveProperty<float>();
        private readonly ReactiveProperty<bool> shooting = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<Vector3> targetPoint = new ReactiveProperty<Vector3>();
        private readonly ReactiveProperty<bool> cameraMove = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<float> cameraZoomDelta = new ReactiveProperty<float>();
        private readonly ReactiveProperty<Vector2> cameraMoveDelta = new ReactiveProperty<Vector2>();

        private Camera PlayerCamera
        {
            get
            {
                if (playerCamera != null)
                {
                    return playerCamera;
                }

                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    Debug.LogWarning("Didn't set player camera for convert coordinaters, use Camera.main by default", gameObject);
                    return mainCamera;
                }

                Debug.LogError("Didn't set camera for convert coordinaters", gameObject);
                return null;
            }
        }

        public IReadonlyReactiveProperty<float> Acceleration => acceleration;
        public IReadonlyReactiveProperty<float> Steering => steering;
        public IReadonlyReactiveProperty<bool> Shooting => shooting;
        public IReadonlyReactiveProperty<Vector3> TargetPoint => targetPoint;
        public IReadonlyReactiveProperty<bool> CameraMove => cameraMove;
        public IReadonlyReactiveProperty<float> CameraZoomDelta => cameraZoomDelta;
        public IReadonlyReactiveProperty<Vector2> CameraMoveDelta => cameraMoveDelta;

        public event Action DoShot;
        public event Action DoSelectPrevWeapon;
        public event Action DoSelectNextWeapon;
        public event Action<int> DoSelectWeapon;

        protected override void SafeAwake()
        {
            base.SafeAwake();

            shooting.SubscribeChanged(OnShootingChanged).SubscribeToDispose(this);
            cameraMove.SubscribeChanged(OnCameraMoveChanged).SubscribeToDispose(this);
        }

        private void OnCameraMoveChanged(bool isCameraMove)
        {
            if (isCameraMove)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }

        private void OnShootingChanged(bool isShooting)
        {
            if (isShooting)
            {
                DoShot?.Invoke();
            }
        }

        private void Update()
        {
            HandleKeyboardInput();
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            shooting.Value = Input.GetMouseButton(0);
            cameraMove.Value = Input.GetMouseButton(1);
            cameraZoomDelta.Value = Input.mouseScrollDelta.y;
            
            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (cameraMove.Value)
            {
                cameraMoveDelta.Value = mouseDelta;
            }

            targetPoint.Value = GetTargetWorldPoint();
        }

        private void HandleKeyboardInput()
        {
            acceleration.Value = Input.GetAxis("Vertical");
            steering.Value = Input.GetAxis("Horizontal");

            shooting.Value = Input.GetKey(KeyCode.X); 

            if (Input.GetKeyDown(KeyCode.Q))
            {
                DoSelectPrevWeapon?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                DoSelectNextWeapon?.Invoke();
            }
        }

        private Vector3 GetTargetWorldPoint()
        {
            var mousePos = Input.mousePosition;
            var cam = PlayerCamera;
            if (cam != null)
            {
                mousePos.z = cam.farClipPlane;
                return cam.ScreenToWorldPoint(mousePos); 
            }

            return default;
        }
    }
}