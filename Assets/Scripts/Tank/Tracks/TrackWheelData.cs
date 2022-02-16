using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

namespace TankShooter.Battle.TankCode
{
    public class TrackWheelData : MonoBehaviour
    {
        [SerializeField] private WheelCollider wheelCollider;
        [SerializeField] private Transform wheelTranform;
        [SerializeField] private bool isDriveWheel;
        [SerializeField] private bool isSlothWheel;

        public float rotationAngle = 0f;

        public WheelCollider WheelCollider => wheelCollider;
        public Transform WheelTransform => wheelTranform;
        public bool IsDriveWheel => isDriveWheel;
        public bool IsSlothWheel => isSlothWheel;
    }
}