using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TankShooter.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankShooter.Battle
{
    public class CameraController : NotifiableMonoBehaviour,
        ICameraInputControllerHandler
    {
        [FormerlySerializedAs("camera")]
        [Header("...references")]
        [SerializeField] 
        private Camera targetCamera;
        [SerializeField] 
        private float cameraSmoothSpeed;
        [SerializeField] 
        private Transform targetTransform;
        [SerializeField] 
        private Transform cameraHolderTransform;

        [Header("...camera move")] 
        [SerializeField]
        private float moveSmoothTime = 0.4f;

        [SerializeField] private float cameraSensetive = 1f;
        
        [Header("...zoom settings")] 
        [SerializeField]
        private float zoomSensetive = 5f;
        [SerializeField] 
        private float zoomDefaultValue = 0.5f;
        [SerializeField]
        private float zoomMinDistance = 5f;
        [SerializeField] 
        private float zoomMaxDistance = 15f;
        [SerializeField]
        private float zoomSmoothTime = 0.2f;

        [SerializeField] private float minCamAngleX = 20f;
        [SerializeField] private float maxCamAngleX = 20f;

        private float zoomTargetValue;
        private float zoomValue;
        private float zoomDistance;

        private float cameraAngleX;
        private float cameraAngleY;

        private void Start()
        {
            ComputeCameraZoom(1);
            ComputeCameraRotationAndTranslation(1);
        }

        private void LateUpdate()
        {
            var dt = Time.smoothDeltaTime;
            ComputeCameraZoom(dt * zoomSmoothTime);
            ComputeCameraRotationAndTranslation(dt * moveSmoothTime);
        }

        public void BindInputController(ICameraInputController inputController)
        {
            inputController.CameraMoveDelta.SubscribeChanged(OnCameraMoveDeltaChanged, true).SubscribeToDispose(this);
            inputController.CameraZoomDelta.SubscribeChanged(OnCameraZoomDeltaChanged).SubscribeToDispose(this);
        }

        private void ComputeCameraZoom(float deltaTime)
        {
            zoomValue = Mathf.Lerp(zoomValue, zoomTargetValue, deltaTime); // считаем зум по колесику мыши
            zoomDistance = Mathf.Lerp(zoomMinDistance, zoomMaxDistance, zoomValue); //считаем реальное расстояние, на которое нужно двигать камеру
        }
        
        private void ComputeCameraRotationAndTranslation(float deltaTime)
        {
            var cameraPosition = transform.position;
            var targetPosition = targetTransform.position;

            //двигаем таргет, куда смотрит камера за танком
            transform.position = Vector3.Lerp(cameraPosition, targetPosition, deltaTime);
            
            //считаем направление камеры, куда ее нужно сдвинуть относительно точки наблюдения
            var forward = Quaternion.Euler(cameraAngleX, cameraAngleY, 0) * new Vector3(0, 0, 1);
            
            //двигаем камеру плавно по времени, она будет как бы бесконечно приближаться к таргет значению
            cameraHolderTransform.localPosition = -forward * zoomDistance;
            cameraHolderTransform.forward = forward;
        }

        private void OnCameraMoveDeltaChanged(Vector2 delta)
        {
            cameraAngleX = Mathf.Clamp(cameraAngleX + delta.y * cameraSensetive, minCamAngleX, maxCamAngleX);
            cameraAngleY = Mathf.Repeat(cameraAngleY + delta.x * cameraSensetive, 360);
        }

        private void OnCameraZoomDeltaChanged(float delta)
        {
            zoomTargetValue = Mathf.Clamp01(zoomTargetValue - delta * zoomSensetive);
        }
    }
}