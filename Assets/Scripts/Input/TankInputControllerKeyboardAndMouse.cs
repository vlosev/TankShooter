using TankShooter.Common;
using UnityEngine;
using UnityEngine.UI;

namespace TankShooter.GameInput
{
    public class TankInputControllerKeyboardAndMouse : InputController
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float maxDistancePhysicsRaycast = 100f;
        [SerializeField] private float maxDistanceRaycastToPlane = 20f;
        [SerializeField] private LayerMask layerMaskForTarget;

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
                var ray = cam.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out var hit, maxDistancePhysicsRaycast, layerMaskForTarget))
                {
                    Debug.DrawLine(cam.transform.position, hit.point, Color.red, 0, false);
                    return hit.point;
                }

                var plane = new Plane(-Vector3.forward, cam.transform.position + Vector3.forward * maxDistanceRaycastToPlane);
                if (plane.Raycast(ray, out var enter))
                {
                    Debug.DrawLine(cam.transform.position, hit.point, Color.red, 0, false);
                    return ray.GetPoint(enter);
                }
            }

            return default;
        }
    }
}