using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankShooter.Tank.Weapon.HommingMissile
{
    public class HomingMissileSlot : MonoBehaviour
    {
        [SerializeField] private Transform missilePivot;

        public Transform MissilePivot => missilePivot;
    }
}