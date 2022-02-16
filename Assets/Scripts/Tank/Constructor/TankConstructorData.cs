using System;
using TankShooter.Battle;
using TankShooter.Battle.TankCode;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Constructor
{
    [CreateAssetMenu(menuName = "TankShooter/Create Tank Constructor Data", fileName = "tank_constructor_data.asset")]
    public class TankConstructorData : ScriptableObject
    {
        [Serializable]
        public struct TankWeaponData
        {
            [SerializeField] private TankWeaponSlotName slotName;
            public TankWeaponSlotName SlotName => slotName;
            
            [SerializeField] private TankWeaponBase[] weapons;
            public TankWeaponBase[] Weapons => weapons;
        }
        
        [Serializable]
        public struct TankModules
        {
            [SerializeField] private GameObject[] bodies;
            public GameObject[] Bodies => bodies;
            
            [SerializeField] private GameObject[] turrets;
            public GameObject[] Turrets => turrets;
            
            [SerializeField] private GameObject[] chassis;
            public GameObject[] Chassis => chassis;
        }

        [SerializeField] private TankModules modules;
        [SerializeField] private TankWeaponData[] weaponData;

        public GameObject GetTankBodyPrefab(int id)
        {
            return modules.Bodies[id];
        }

        public GameObject GetTankTurretPrefab(int id)
        {
            return modules.Turrets[id];
        }

        public GameObject GetTankChassisPrefab(int id)
        {
            return modules.Chassis[id];
        }
        
        public TankWeaponBase GetWeaponPrefab(TankWeaponSlotName slotName, int id)
        {
            foreach (var item in weaponData)
                if (item.SlotName == slotName)
                    return item.Weapons[id];
            
            return null;
        }
    }
}