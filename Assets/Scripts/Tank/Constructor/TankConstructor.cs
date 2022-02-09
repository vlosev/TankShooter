using System;
using System.Linq;
using Common;
using Tank.Interfaces;
using TankShooter.Battle;
using TankShooter.Battle.Tank;
using TankShooter.Battle.TankCode;
using UnityEngine;

namespace TankShooter.Tank.Constructor
{
    public class TankConstructor : MonoBehaviour
    {
        [SerializeField] private TankConstructorData constructorData;
        
        public GameObject CreateTank(Transform spawnPoint, int tankId, int chassisId, int turretId, (TankWeaponSlotName slot, int index)[] weapons)
        {
            var bodyPrefab = constructorData.GetTankBodyPrefab(tankId);
            if (bodyPrefab == null)
                throw new Exception($"Tank body '{tankId}' is null!");

            var chassisPrefab = constructorData.GetTankChassisPrefab(chassisId);
            if (chassisPrefab == null)
                throw new Exception($"Tank chassis '{chassisId}' is null!");
            
            var turretPrefab = constructorData.GetTankTurretPrefab(turretId);
            if (turretPrefab == null)
                throw new Exception($"Tank turret '{turretPrefab}' is null!");

            var body = Instantiate(bodyPrefab, spawnPoint.position, spawnPoint.rotation);
            if (body != null)
            {
                if (body.TryGetComponent<TankConstructorBodyHelper>(out var constructor))
                {
                    Instantiate(chassisPrefab, constructor.ChassisPivot);
                    Instantiate(turretPrefab, constructor.TurretPivot);
                }
                
                //check weapon slots
                if (body.TryGetComponentsInChildren<TankWeaponSlot>(out var slots))
                {
                    foreach (var weaponItem in weapons)
                    {
                        var slot = slots.FirstOrDefault(i => i.SlotName == weaponItem.slot);
                        var weaponPrefab = constructorData.GetWeaponPrefab(weaponItem.slot, weaponItem.index);
                        if (slot != null && weaponPrefab != null)
                        {
                            var instance = Instantiate(weaponPrefab, slot.transform, false);
                            instance.transform.localPosition = Vector3.zero;
                            instance.transform.localRotation = Quaternion.identity;
                            
                            slot.SetWeapon(instance);
                        }
                    }
                }

                return body;
            }

            Debug.LogError($"Can't create tank with body: {tankId}, chassis: {chassisId}, turret: {turretId}, " +
                           $"weapons: [{string.Join(", ", weapons.Select(i => $"weapon: {i.slot} / {i.index}"))}]");
            return null;
        }
    }
}