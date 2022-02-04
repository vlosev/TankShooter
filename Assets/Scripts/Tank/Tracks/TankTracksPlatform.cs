using System;
using System.Collections;
using System.Collections.Generic;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Battle.TankCode
{
    public class TankTracksPlatform : NotifiableMonoBehaviour, IPhysicsBeforeTickListener, ITankModule
    {
        [Header("...physics settings")] 
        [SerializeField] private float minBrakeTorque = 100f;
        [SerializeField] private float maxBrakeTorque = 1500f;
        [SerializeField] private float forwardMaxTorque = 1000f;
        [SerializeField] private float rotateTorqueMultiplyOnMove = 0.4f;
        [SerializeField] private float reverseMaxTorque = 300f;
        [SerializeField] private float rotateOnStandMotorTorque = 500f;
        [SerializeField] private float rotateOnStandBrakeTorque = 50f;
        [SerializeField] private float minStifnessOfStay = 0.1f;
        [SerializeField] private float minStifnessOfMove = 0.1f;
        
        [SerializeField] private TankTrack LTrack;
        [SerializeField] private TankTrack RTrack;

        //reference to parent
        private ITank tank;
        private IInputController inputController;
        private Rigidbody rigidbody;
        
        private float acceleration;
        private float steering;

        protected override void SafeAwake()
        {
            base.SafeAwake();
            
            BattleTimeMachine.SubscribePhysicsBeforeTick(this).SubscribeToDispose(this);
        }
        
        public void SetupModule(ITank tank)
        {
            this.tank = tank;
            this.rigidbody = tank.Rigidbody;
            
            tank.InputContoller.Acceleration.SubscribeChanged(value => acceleration = value, true).SubscribeToDispose(this);
            tank.InputContoller.Steering.SubscribeChanged(value => steering = value, true).SubscribeToDispose(this);
        }

        public void OnBeforePhysicsTick(float dt)
        {
            var isAccel = !Mathf.Approximately(acceleration, 0f);
            var isSteer = !Mathf.Approximately(steering, 0f);

            if (isAccel && isSteer)
            {
                AcceleratingAndSteering();
            }
            else if (isAccel)
            {
                Accelerating();
            }
            else if (isSteer)
            {
                SteeringOnStand();
            }
            else
            {
                StopTrackWheels();
            }
        }

        private void StopTrackWheels()
        {
            LTrack.SetTorques(maxBrakeTorque, 0f, minStifnessOfStay);
            RTrack.SetTorques(maxBrakeTorque, 0f, minStifnessOfStay);
        }

        private void Accelerating()
        {
            var motorTorque = ComputeMotorTorque();
            var stiffnes = ComputeSidewayStiffnes(false);
            
            LTrack.SetTorques(minBrakeTorque, motorTorque, stiffnes);
            RTrack.SetTorques(minBrakeTorque, motorTorque, stiffnes);
        }

        private void AcceleratingAndSteering()
        {
            var motorTorque = ComputeMotorTorque();
            var stiffnes = ComputeSidewayStiffnes(false);

            var lTorque = motorTorque * rotateTorqueMultiplyOnMove * steering;
            var rTorque = motorTorque * rotateTorqueMultiplyOnMove * -steering;
            
            LTrack.SetTorques(minBrakeTorque, lTorque, stiffnes);
            RTrack.SetTorques(minBrakeTorque, rTorque, stiffnes);
            
            // Debug.Log($"track accelerating and steering, motorTorque = {motorTorque}, steer = {steering}");
        }

        private void SteeringOnStand()
        {
            var lTorque = steering * rotateOnStandMotorTorque;
            var rTorque = -steering * rotateOnStandMotorTorque;
            var stiffnes = ComputeSidewayStiffnes(true);
            
            LTrack.SetTorques(rotateOnStandBrakeTorque, lTorque, stiffnes);
            RTrack.SetTorques(rotateOnStandBrakeTorque, rTorque, stiffnes);

            // Debug.Log($"STEERING ON STAND: brake: {rotateOnStandBrakeTorque}, lTorque: {lTorque}, rTorque: {rTorque}");
        }

        //получаем крутящий момент для разных направлений
        private float ComputeMotorTorque()
        {
            return acceleration > 0f
                ? forwardMaxTorque * acceleration
                : reverseMaxTorque * acceleration;
        }

        private float ComputeSidewayStiffnes(bool isStay)
        {
            return 1.0f + (isStay ? minStifnessOfStay : minStifnessOfMove) - Mathf.Abs(steering);
        }
        
        //вычисляем среднюю скорость колес
        private float GetAverageRPM()
        {
            var wheelsCount = 0;
            var sumRpm = 0f;

            int trackWheelsCount;
            float trackSumRpm;
            LTrack.GetRpmAndWheelsCount(out trackWheelsCount, out trackSumRpm);
            wheelsCount += trackWheelsCount;
            sumRpm += trackSumRpm;

            RTrack.GetRpmAndWheelsCount(out trackWheelsCount, out trackSumRpm);
            wheelsCount += trackWheelsCount;
            sumRpm += trackSumRpm;

            return wheelsCount != 0 ? sumRpm / wheelsCount : 0f;
        }
    }
}