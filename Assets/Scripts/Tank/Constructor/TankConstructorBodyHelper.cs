using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankShooter.Tank.Constructor
{
    public class TankConstructorBodyHelper : MonoBehaviour
    {
        [SerializeField] private Transform turretPivot;
        public Transform TurretPivot => turretPivot;

        [SerializeField] private Transform chassisPivot;
        public Transform ChassisPivot => chassisPivot;
    }
}