using System;
using System.Collections;
using System.Collections.Generic;
using Tank.Interfaces;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Battle.TankCode
{
    public class TankTurretController :
        NotifiableMonoBehaviour,
        ITankModule,
        ITankInputControllerHandler
    {
        [Tooltip("скорость вращения башни в градусов/сек")]
        [SerializeField] private float rotationTurrentSpeed = 20f;
        [Tooltip("минимальный угол наклона пушки в градусах")]
        [SerializeField] private float minGunAngle = -20f;
        [Tooltip("максимальный угол наклона пушки в градусах")]
        [SerializeField] private float maxGunAngle = 20f;
        [Tooltip("скорость наклона/поднятия пушки градусов/сек")] 
        [SerializeField] private float turnGunSpeed = 10f;

        [SerializeField] private Transform turretPivot;
        [SerializeField] private Transform gunPivot;
        [SerializeField] private Transform gunShotPosition;

        private Vector3 targetWorldPoint;
        
        private Quaternion turretTargetRotation;
        private Quaternion gunTargetRotation;

        private void Update()
        {
            var deltaTime = Time.deltaTime;
            
            //получаем позицию курсора в мире, чтобы целиться в эту точку
            UpdateTurretDirection(targetWorldPoint, deltaTime);
            UpdateGunDirection(targetWorldPoint, deltaTime);
        }
        
        public void Init(ITank tank)
        {
            //do nothing
        }

        public void BindInputController(ITankInputController tankInputController)
        {
            tankInputController.TargetPoint.SubscribeChanged(point => targetWorldPoint = point).SubscribeToDispose(this);
        }

        private void UpdateTurretDirection(Vector3 targetWorldPoint, float dt)
        {
            //находим точку, куда относительно башни смотрим и вращаем башню туда
            var currentRotation = turretPivot.localRotation;
            var currentPosition = turretPivot.transform.position;
            targetWorldPoint.y = currentPosition.y; //башню поворачиваем в плоскости XoZ
            
            var targetDirection = (targetWorldPoint - currentPosition).normalized;
            var localDirection = transform.InverseTransformDirection(targetDirection);
            if (localDirection.sqrMagnitude > 0f)
            {
                turretTargetRotation.SetLookRotation(localDirection, turretPivot.up);
                currentRotation = Quaternion.RotateTowards(currentRotation, turretTargetRotation, dt * rotationTurrentSpeed);
                currentRotation.eulerAngles = new Vector3(0, currentRotation.eulerAngles.y, 0); //убираем накапливаемую ошибку
                turretPivot.localRotation = currentRotation;

#if UNITY_EDITOR
                Debug.DrawLine(currentPosition, currentPosition + turretPivot.forward * 10f, Color.yellow, 0, false);
                Debug.DrawLine(currentPosition, currentPosition + targetDirection * 10f, Color.green, 0, false);
#endif
            }
        }

        private void UpdateGunDirection(Vector3 targetWorldPoint, float dt)
        {
            var currentPosition = gunPivot.position;
            
            //1) получаем вектор от точки в мире до точки, куда сейчас смотрит оружие и дистанцию до этой точки
            var fromGunPositionToTargetPoint = targetWorldPoint - currentPosition;
            var fromGunPositionToTargetPointDistance = fromGunPositionToTargetPoint.magnitude;
            
            //2) теперь отодвигаемся от позиции пивота оружия по вектору forward в том же направлении и задаем высоту точки прицеливания
            var gunTargetPoint = gunPivot.position + gunPivot.forward * fromGunPositionToTargetPointDistance;
            gunTargetPoint.y = targetWorldPoint.y;
            
            //находим точку, куда относительно башни должно быть направлено оружие и вращаем туда
            var currentRotation = gunPivot.localRotation;
            var targetDirection = (gunTargetPoint - currentPosition).normalized;
            var localDirection = turretPivot.InverseTransformDirection(targetDirection);
            if (localDirection.sqrMagnitude > 0f)
            {
                gunTargetRotation.SetLookRotation(localDirection, turretPivot.up);

                var targetQuat = Quaternion.RotateTowards(currentRotation, gunTargetRotation, dt * turnGunSpeed);
                targetQuat.Normalize();

                //клампим кватернион, чтобы нельзя было слишком сильно поднять и опустить пушку
                var eulerAngles = targetQuat.eulerAngles;
                //Debug.Log($"ea: {eulerAngles}");
                //eulerAngles.x = Mathf.Clamp(eulerAngles.x, Mathf.Repeat(minGunAngle, 360), Mathf.Repeat(maxGunAngle, 360));
                targetQuat.eulerAngles = eulerAngles;
                
                gunPivot.localRotation = targetQuat;

                Debug.DrawLine(gunPivot.position, gunTargetPoint, Color.red, 0, false);
            }
        }
    }
}