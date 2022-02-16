using Common.MathUtils;
using TankShooter.Common;
using TankShooter.GameInput;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Tank
{
    public class TankTurretControllerContext
    {
    }
    
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
        [SerializeField] private Transform gunPivot;
        
        private TankWeaponSlot gunSlot;
        private Transform selfTransform;
        private Transform tankTransform;

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
            tankTransform = tank.Transform;
            selfTransform = transform;
        }

        public void BindInputController(ITankInputController tankInputController)
        {
            tankInputController.TargetPoint.SubscribeChanged(point => targetWorldPoint = point).SubscribeToDispose(this);
        }

        private void UpdateTurretDirection(Vector3 targetWorldPoint, float dt)
        {
            //находим точку, куда относительно башни смотрим и вращаем башню туда
            var currentRotation = selfTransform.localRotation;
            var currentPosition = selfTransform.transform.position;
            targetWorldPoint.y = currentPosition.y; //башню поворачиваем в плоскости XoZ
            
            var targetDirection = (targetWorldPoint - currentPosition).normalized;
            var localDirection = tankTransform.InverseTransformDirection(targetDirection);
            if (localDirection.sqrMagnitude > 0f)
            {
                turretTargetRotation.SetLookRotation(localDirection, selfTransform.up);
                currentRotation = Quaternion.RotateTowards(currentRotation, turretTargetRotation, dt * rotationTurrentSpeed);
                currentRotation.eulerAngles = new Vector3(0, currentRotation.eulerAngles.y, 0); //убираем накапливаемую ошибку
                selfTransform.localRotation = currentRotation;

                Debug.DrawLine(currentPosition, currentPosition + selfTransform.forward * 10f, Color.yellow, 0, false);
                Debug.DrawLine(currentPosition, currentPosition + targetDirection * 10f, Color.green, 0, false);
            }
        }

        private void UpdateGunDirection(Vector3 targetWorldPoint, float dt)
        {
            var sourcePos = gunPivot.position;
            var turretDir = selfTransform.forward;

            //1) получаем вектор от точки в мире до точки, куда сейчас смотрит оружие и дистанцию до этой точки
            var fromGunPositionToTargetPoint = targetWorldPoint - sourcePos;
            var fromGunPositionToTargetPointDistance = fromGunPositionToTargetPoint.magnitude;
            
            //2) теперь отодвигаемся от позиции пивота оружия по вектору forward в том же направлении и задаем высоту точки прицеливания
            var targetPos = sourcePos + turretDir * fromGunPositionToTargetPointDistance;
            targetPos.y = targetWorldPoint.y;
            
            //находим точку, куда относительно башни должно быть направлено оружие и вращаем туда
            var currentRotation = gunPivot.localRotation;
            var targetDirection = (targetPos - sourcePos).normalized;
            var localDirection = selfTransform.InverseTransformDirection(targetDirection);
            if (localDirection.sqrMagnitude > 0f)
            {
                gunTargetRotation.SetLookRotation(localDirection, selfTransform.up);
            }

            var targetQuat = Quaternion.RotateTowards(currentRotation, gunTargetRotation, dt * turnGunSpeed);
            targetQuat.Normalize();
            var eulerAngles = targetQuat.eulerAngles;
            eulerAngles.x = MathUtils.ClampAngle(eulerAngles.x, minGunAngle, maxGunAngle);
            eulerAngles.z = 0f;
            eulerAngles.y = 0f;
            targetQuat.eulerAngles = eulerAngles;
            gunPivot.localRotation = targetQuat;

            Debug.DrawLine(sourcePos, sourcePos + currentRotation * turretDir * 10f, Color.magenta, 0, false);
            Debug.DrawLine(sourcePos, sourcePos + targetDirection * 5f, Color.yellow, 0, false);
            Debug.DrawLine(sourcePos, sourcePos + turretDir * 5f, Color.green, 0, false);
        }
    }
}