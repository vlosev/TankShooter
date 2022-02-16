using System;
using System.Collections.Generic;
using System.Linq;
using TankShooter.Battle.TankCode;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Tank
{
    public class TankTrack : NotifiableMonoBehaviour
    {
        private float wheelsRotationAngle = 0f;
        
        [SerializeField] private TrackWheelData[] wheelsData;

        public TrackWheelData[] WheelsData => wheelsData;

        private void Update()
        {
            //update visual rotation
            var dt = Time.deltaTime;
            var averageRpm = GetRpmAndWheelsCount();

            wheelsRotationAngle = Mathf.Repeat(wheelsRotationAngle + averageRpm * dt * 360.0f / 60.0f, 360f);
            var rotationQuat = Quaternion.Euler(wheelsRotationAngle, 0, 90);
            foreach (var wd in wheelsData)
            {
                wd.rotationAngle = Mathf.Repeat(wd.rotationAngle + averageRpm * dt * 360.0f / 60.0f, 360f);
                
                wd.WheelCollider.GetWorldPose(out var position, out var rotation);
                wd.WheelTransform.position = position;
                wd.WheelTransform.localRotation = rotationQuat;
            }
        }
        
        public float GetRpmAndWheelsCount()
        {
            var wheelsCount = 0;
            var rpm = 0f;

            foreach (var wd in wheelsData)
            {
                var wc = wd.WheelCollider;
                
                //считаем обороты для визуализации только с тех колес, которые обычно на земле
                //два других обычно в свободном вращении и будут портить результат
                
                if (wc.isGrounded)
                {
                    rpm += wc.rpm;
                    wheelsCount++;
                }
            }
            
            return wheelsCount != 0 ? rpm / wheelsCount : 0;
        }

        public void SetTorques(float brakeTorque, float motorTorque, float sidewaysStiffness, float forwardStiffness)
        {
            WheelFrictionCurve frictionCurve;
            for (int i = 0; i < wheelsData.Length; ++i)
            {
                var wheelCollider = wheelsData[i].WheelCollider;
                
                // SetSidewaysStiffness(wheelCollider, sidewaysStiffness);
                // SetForwardStiffness(wheelCollider, forwardStiffness);

                var stop = false;
                var rpm = wheelCollider.rpm;
                if (motorTorque != 0f)
                {
                    if (motorTorque > 0f && rpm < 0f || motorTorque < 0f && rpm > 0f)
                        stop = true;
                }
                
                if (stop)
                {
                    wheelCollider.brakeTorque = motorTorque;
                }
                else
                {
                    wheelCollider.brakeTorque = brakeTorque;
                    wheelCollider.motorTorque = motorTorque;
                }
            }
        }


    }
}