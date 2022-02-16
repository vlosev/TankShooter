using System;
using System.Linq;
using Common.PhysicsUtils;
using TankShooter.Common;
using TankShooter.Common.FSM;
using TankShooter.GameInput;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankShooter.Tank
{
    public partial class TankChassis : NotifiableMonoBehaviour,
        IPhysicsBeforeTickListener,
        ITankModule,
        ITankInputControllerHandler
    {
        #region inspector
        [Header("...move settings")]
        [SerializeField] private float maxSpeedKmh = 60f;
        [SerializeField] private AnimationCurve torqueCurve;
        [SerializeField] private float brakeCoeffOfSpeedLimit = 0.1f;

        [Header("...physics settings")] 
        [SerializeField] private float minBrakeTorque = 100f;
        [SerializeField] private float maxBrakeTorque = 1500f;
        [SerializeField] private float forwardMaxTorque = 1000f;
        [SerializeField] private float rotateTorqueMultiplyOnMove = 0.4f;
        [SerializeField] private float reverseMaxTorque = 300f;
        [SerializeField] private float rotateOnStandMotorTorque = 500f;
        [SerializeField] private float rotateOnStandBrakeTorque = 50f;
        [SerializeField] private float rotateOnMoveBrakeTorque = 50f;
        [SerializeField] private float centerOfMassHeight = -2f;

        [Header("Настройки трения при развороте на месте")]
        [SerializeField] private float minForwardStiffnessOfStay = 0.8f;
        [SerializeField] private float maxForwardStiffnessOfStay = 1f;
        [SerializeField] private float minSidewayStiffnessOfStay = 0.1f;
        [SerializeField] private float maxSidewayStiffnessOfStay = 0.1f;
        
        [Header("Настройки трения при движении")]
        [SerializeField] private float minForwardStiffnessOfMove = 0.1f;
        [SerializeField] private float maxForwardStiffnessOfMove = 0.3f;
        [SerializeField] private float minSidewayStiffnessOfMove = 0.1f;
        [SerializeField] private float maxSidewayStiffnessOfMove = 0.1f;

        [SerializeField] private float minSpeedForMove = 1f;
        #endregion

        [SerializeField] private TankTrack LTrack;
        [SerializeField] private TankTrack RTrack;

        private Rigidbody rigidbody;
        private event Action<float> OnPhysicsTickEvent;

        private Fsm<TankChassis> fsm;
        private WheelCollider[] leftWheelColliders;
        private WheelCollider[] rightWheelColliders;

        private float speed;
        private float acceleration;
        private float steering;

        protected override void SafeAwake()
        {
            base.SafeAwake();
            BattleTimeMachine.SubscribePhysicsBeforeTick(this).SubscribeToDispose(this);
        }
        
        public void Init(ITank tank)
        {
            this.rigidbody = tank.Rigidbody;

            leftWheelColliders = LTrack.WheelsData.Select(i => i.WheelCollider).ToArray(); 
            rightWheelColliders = RTrack.WheelsData.Select(i => i.WheelCollider).ToArray(); 
            fsm = new Fsm<TankChassis>(new StateStop(this));

            ComputeCenterOfMass();
        }

        public void BindInputController(ITankInputController tankInputController)
        {
            tankInputController.Acceleration.SubscribeChanged(value =>
            {
                acceleration = value;
                fsm?.Update();
            }, true).SubscribeToDispose(this);

            tankInputController.Steering.SubscribeChanged(value =>
            {
                steering = value;
                fsm?.Update();
            }, true).SubscribeToDispose(this);
        }

        void IPhysicsBeforeTickListener.OnBeforePhysicsTick(float dt)
        {
            //compute stuff
            speed = PhysicsUtils.ConvertSpeedMStoKMH(rigidbody.velocity.magnitude);
            OnPhysicsTickEvent?.Invoke(dt);
        }

        private void ComputeCenterOfMass()
        {
            var centerOfMass = new GameObject("CENTER_OF_MASS");
            centerOfMass.transform.SetParent(transform);

            var wheelsCount = 0;
            var centerOfMassPosition = Vector3.zero;
            foreach (var wd in LTrack.WheelsData)
            {
                centerOfMassPosition += wd.WheelCollider.transform.localPosition;
                wheelsCount++;
            }

            foreach (var wd in LTrack.WheelsData)
            {
                centerOfMassPosition += wd.WheelCollider.transform.localPosition;
                wheelsCount++;
            }

            centerOfMassPosition.y = 0f;
            centerOfMassPosition = (centerOfMassPosition / wheelsCount);
            centerOfMassPosition.y = centerOfMassHeight;
            
            centerOfMass.transform.localPosition = centerOfMassPosition;
            rigidbody.centerOfMass = centerOfMassPosition;
        }
    }
}