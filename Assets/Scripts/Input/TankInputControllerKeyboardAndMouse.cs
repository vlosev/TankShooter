using TankShooter.Common;
using UnityEngine;

namespace TankShooter.GameInput
{
    public class TankInputControllerKeyboardAndMouse : InputController
    {
        [SerializeField] private Camera playerCamera;

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

        protected override void SafeAwake()
        {
            base.SafeAwake();

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

        private void Update()
        {
            GetShotButton();
            HandleKeyboardInput();
            HandleMouseInput();
        }

        //это особый случай, чтобы не менять свойство дважды, хендлится так
        private void GetShotButton()
        {
            shooting.Value = Input.GetKey(KeyCode.X) || Input.GetMouseButton(0);
        }

        private void HandleMouseInput()
        {
            cameraMove.Value = Input.GetMouseButton(1);
            cameraZoomDelta.Value = Input.mouseScrollDelta.y;
            
            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (cameraMove.Value)
            {
                cameraMoveDelta.Value = mouseDelta;
            }
            else
            {
                targetPoint.Value = GetTargetWorldPoint();
            }
        }

        private void HandleKeyboardInput()
        {
            acceleration.Value = Input.GetAxis("Vertical");
            steering.Value = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Q))
            {
                DoSelectPrevWeapon();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                DoSelectNextWeapon();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                DoReloadingWeapon();
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