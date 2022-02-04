using System;
using System.Collections.Generic;
using System.Linq;
using TankShooter.Battle.TankCode;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Battle
{
    public class TankTrack : NotifiableMonoBehaviour
    {
        private readonly Quaternion CHANGE_WHEEL_ORIENTATION = Quaternion.Euler(0, 0, 90);
        
        [SerializeField] private TrackWheelData[] wheelsData;

        public TrackWheelData[] WheelsData => wheelsData;

        private void Update()
        {
            for (var i = 0; i < wheelsData.Length; ++i)
            {
                var wd = wheelsData[i];
                
                wd.WheelCollider.GetWorldPose(out var position, out var rotation);
                wd.WheelTransform.position = position;
                wd.WheelTransform.rotation = rotation * CHANGE_WHEEL_ORIENTATION;
            }
        }

        public void GetRpmAndWheelsCount(out int wheelsCount, out float rpm)
        {
            wheelsCount = 0;
            rpm = 0;
            
            foreach (var wd in wheelsData)
            {
                var wc = wd.WheelCollider;
                if (wc.isGrounded)
                {
                    wheelsCount++;
                    rpm += wc.rpm;
                }
            }
        }

        public void SetTorques(float brakeTorque, float motorTorque, float sidewaysFriction)
        {
            //clamp sideway stiffnes
            sidewaysFriction = Mathf.Min(sidewaysFriction, 1);
            
            for (int i = 0; i < wheelsData.Length; ++i)
            {
                var wheelCollider = wheelsData[i].WheelCollider;
                wheelCollider.brakeTorque = brakeTorque;
                wheelCollider.motorTorque = motorTorque;
                
                var sf = wheelCollider.sidewaysFriction;
                sf.stiffness = sidewaysFriction;
                wheelCollider.sidewaysFriction = sf;
            }
        }
    }
}