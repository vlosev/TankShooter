using TankShooter.Common.FSM;
using UnityEngine;

namespace TankShooter.Tank
{
    public partial class TankChassis
    {
        protected abstract class TankChassisState : FsmState<TankChassis>
        {
            protected enum InputState { Stop, Acceleration, Steering, Both }

            protected readonly WheelCollider[] leftWheelColliders;
            protected readonly WheelCollider[] rightWheelColliders;

            protected TankChassisState(TankChassis entity) : base(entity)
            {
                leftWheelColliders = entity.leftWheelColliders;
                rightWheelColliders = entity.rightWheelColliders;
            }

            public override void OnEnter()
            {
                SetupWheelColliders();
                base.OnEnter();
            }

            public override FsmState<TankChassis> Update()
            {
                switch (GetInputState())
                {
                    case InputState.Stop:
                        return new StateStop(entity);
                    case InputState.Steering:
                        return new StateSteeringStay(entity);
                    case InputState.Both:
                        return new StateSteeringMove(entity);
                    case InputState.Acceleration:
                        return new StateMove(entity);
                }
                
                SetupWheelColliders();
                return base.Update();
            }

            public override void OnLeave()
            {
                ResetWheelColliders();
                base.OnLeave();
            }

            protected InputState GetInputState()
            {
                if (entity.acceleration != 0f && entity.steering != 0f)
                    return InputState.Both;
                if (entity.acceleration != 0f) 
                    return InputState.Acceleration;
                if (entity.steering != 0f) 
                    return InputState.Steering;
                return InputState.Stop;
            }

            protected virtual void SetupWheelColliders()
            {
            }

            protected virtual void ResetWheelColliders()
            {
                void ResetWheelCollider(WheelCollider wc)
                {
                    wc.motorTorque = 0f;
                    wc.brakeTorque = 0f;
                }
                
                foreach (var wc in leftWheelColliders)
                    ResetWheelCollider(wc);
                
                foreach (var wc in rightWheelColliders)
                    ResetWheelCollider(wc);
            }
        }

        protected class StateMove : TankChassisState
        {
            private readonly float minSidewaysFriction;
            private readonly float minForwardFriction;
            private readonly float maxSidewaysFriction;
            private readonly float maxForwardFriction;

            private float sidewaysFriction;
            private float forwardFriction;
            private float maxAccelMotorTorque;
            private float motorTorque;
            
            public StateMove(TankChassis entity) : base(entity)
            {
                minSidewaysFriction = entity.minSidewayStiffnessOfMove;
                minForwardFriction = entity.minForwardStiffnessOfMove;
                maxSidewaysFriction = entity.maxSidewayStiffnessOfMove;
                maxForwardFriction = entity.maxForwardStiffnessOfMove;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                entity.OnPhysicsTickEvent += OnPhysicsTick;
            }

            public override void OnLeave()
            {
                entity.OnPhysicsTickEvent -= OnPhysicsTick;
                base.OnLeave();
            }

            private void OnPhysicsTick(float obj)
            {
                motorTorque = GetClampedMotorTorque(maxAccelMotorTorque);
                foreach (var wc in leftWheelColliders)
                    wc.motorTorque = motorTorque;
                foreach (var wc in rightWheelColliders)
                    wc.motorTorque = motorTorque;
            }

            protected override void SetupWheelColliders()
            {
                sidewaysFriction = Mathf.Lerp(minSidewaysFriction, maxSidewaysFriction, Mathf.Abs(entity.steering));
                forwardFriction = Mathf.Lerp(minForwardFriction, maxForwardFriction, Mathf.Abs(entity.acceleration));

                //берем нужный крутящий момент в зависимости от знака
                maxAccelMotorTorque = entity.acceleration * (entity.acceleration > 0f ? entity.forwardMaxTorque : entity.reverseMaxTorque);
                motorTorque = GetClampedMotorTorque(maxAccelMotorTorque);
                foreach (var wc in leftWheelColliders)
                {
                    wc.motorTorque = motorTorque;
                    wc.brakeTorque = 0f;
                    entity.SetSidewaysStiffness(wc, sidewaysFriction);
                    entity.SetForwardStiffness(wc, forwardFriction);
                }

                foreach (var wc in rightWheelColliders)
                {
                    wc.motorTorque = motorTorque;
                    wc.brakeTorque = 0f;
                    entity.SetSidewaysStiffness(wc, sidewaysFriction);
                    entity.SetForwardStiffness(wc, forwardFriction);
                }
            }

            private float GetClampedMotorTorque(float motorTorque)
            {
                var t = Mathf.Clamp01(entity.speed / entity.maxSpeedKmh);
                var v = entity.torqueCurve.Evaluate(t);
                return motorTorque * v;
            }
        }

        protected class StateSteeringMove : TankChassisState
        {
            private readonly float minSidewaysFriction;
            private readonly float minForwardFriction;
            private readonly float maxSidewaysFriction;
            private readonly float maxForwardFriction;

            public StateSteeringMove(TankChassis entity) : base(entity)
            {
                minSidewaysFriction = entity.minSidewayStiffnessOfMove;
                minForwardFriction = entity.minForwardStiffnessOfMove;
                maxSidewaysFriction = entity.maxSidewayStiffnessOfMove;
                maxForwardFriction = entity.maxForwardStiffnessOfMove;
            }

            protected override void SetupWheelColliders()
            {
                var sidewaysStiffness = Mathf.Lerp(minSidewaysFriction, maxSidewaysFriction, Mathf.Abs(entity.steering));
                var forwardStiffness = Mathf.Lerp(minForwardFriction, maxForwardFriction, Mathf.Abs(entity.acceleration));
                var motorTorque = entity.acceleration * (entity.acceleration > 0f ? entity.forwardMaxTorque : entity.reverseMaxTorque);

                void SetupCollider(WheelCollider wc, float steer, float sf)
                {
                    wc.brakeTorque = entity.minBrakeTorque;
                    wc.motorTorque = motorTorque;
                    
                    if (steer < 0f)
                    {
                        wc.motorTorque = steer * motorTorque * entity.rotateTorqueMultiplyOnMove - entity.rotateOnMoveBrakeTorque;

                        sf = 1.0f + minSidewaysFriction - Mathf.Abs(steer);
                        entity.SetSidewaysStiffness(wc, sidewaysStiffness);
                    }
                    else if (steer < 0f)
                    {
                        wc.motorTorque = steer * motorTorque * entity.rotateTorqueMultiplyOnMove;
                        sf = 1.0f + minSidewaysFriction - Mathf.Abs(steer);
                    }
                    
                    entity.SetSidewaysStiffness(wc, sf);
                    entity.SetForwardStiffness(wc, forwardStiffness);
                }

                //берем нужный крутящий момент в зависимости от знака
                var steer = entity.steering;
                foreach (var wc in leftWheelColliders)
                    SetupCollider(wc, steer, sidewaysStiffness);
                
                foreach (var wc in rightWheelColliders)
                    SetupCollider(wc, -steer, sidewaysStiffness);
            }
        }

        protected class StateSteeringStay : TankChassisState
        {
            private readonly float minSidewaysFriction;
            private readonly float minForwardFriction;
            private readonly float maxSidewaysFriction;
            private readonly float maxForwardFriction;
            
            public StateSteeringStay(TankChassis entity) : base(entity)
            {
                minSidewaysFriction = entity.minForwardStiffnessOfStay;
                minForwardFriction = entity.minForwardStiffnessOfStay;
                maxSidewaysFriction = entity.maxForwardStiffnessOfStay;
                maxForwardFriction = entity.maxForwardStiffnessOfStay;
            }

            protected override void SetupWheelColliders()
            {
                var absSteering = Mathf.Abs(entity.steering);
                var sidewaysStiffness = Mathf.Lerp(minSidewaysFriction, maxSidewaysFriction, absSteering);
                var forwardStiffness = Mathf.Lerp(minForwardFriction, maxForwardFriction, absSteering);
                
                var leftMotorTorque = entity.steering * entity.rotateOnStandMotorTorque;
                foreach (var wc in leftWheelColliders)
                {
                    wc.motorTorque = leftMotorTorque;
                    wc.brakeTorque = 0f;
                    
                    entity.SetSidewaysStiffness(wc, sidewaysStiffness);
                    entity.SetForwardStiffness(wc, forwardStiffness);
                }

                var rightMotorTorque = -entity.steering * entity.rotateOnStandMotorTorque;
                foreach (var wc in rightWheelColliders)
                {
                    wc.motorTorque = rightMotorTorque;
                    wc.brakeTorque = 0f;
                    
                    entity.SetSidewaysStiffness(wc, sidewaysStiffness);
                    entity.SetForwardStiffness(wc, forwardStiffness);
                }
            }
        }

        protected class StateStop : TankChassisState
        {
            public StateStop(TankChassis entity) : base(entity)
            {
            }

            protected override void SetupWheelColliders()
            {
                foreach (var wc in leftWheelColliders) 
                {
                    wc.motorTorque = 0f;
                    wc.brakeTorque = entity.maxBrakeTorque;
                    
                    entity.SetForwardStiffness(wc, 1);
                    entity.SetSidewaysStiffness(wc, 1);
                }
                
                foreach (var wc in rightWheelColliders)
                {
                    wc.motorTorque = 0f;
                    wc.brakeTorque = entity.maxBrakeTorque;

                    entity.SetForwardStiffness(wc, 1);
                    entity.SetSidewaysStiffness(wc, 1);
                }
            }
        }
    }
}