using System;
using System.Runtime.InteropServices;
using Common;
using Tank.Interfaces;
using Tank.Weapon;
using TankShooter.Common;
using TankShooter.GameInput;
using UnityEngine;

namespace TankShooter.Tank
{
    [Serializable]
    public class TankSettings
    {
        public float MaxSpeed = 500f;
        public float MaxTurnSpeed = 5f;
    }
    
    public class TankController : NotifiableMonoBehaviour,
        ITankInputControllerHandler,
        ITank
    {
        private ITankInputController inputController;

        [Header("Tank refrerences for constructor")] 
        [SerializeField] private Transform turretPivot;
        [SerializeField] private Transform chassisPivot;
        
        [Header("references")]
        [SerializeField] private Rigidbody tankRigidbody;
        [SerializeField] private TankWeaponManager weaponManager;
        
        [Header("gameplay parameters")]
        [SerializeField] private Transform centerOfMass;

        #region ITank implementation
        Transform ITank.Transform => transform;
        Rigidbody ITank.Rigidbody => tankRigidbody;
        ITankInputController ITank.InputController => inputController;
        #endregion

        //TODO: сделать загрузку и настройку
        [SerializeField]
        private TankSettings settings = new TankSettings()
        {
            MaxSpeed = 2000,
            MaxTurnSpeed = 3
        };

        protected override void SafeAwake()
        {
            base.SafeAwake();
            tankRigidbody.centerOfMass = centerOfMass.localPosition;
        }
        
        //сюда передаем какие-нибудь настройки танка
        public void InitTank(/* ... */)
        {
            var modules = GetComponentsInChildren<ITankModule>(true);
            foreach (var module in modules)
            {
                module.Init(this);
            }
        }

        public void BindInputController(ITankInputController tankInputController)
        {
            inputController = tankInputController;

            var inputControllerHandlers = GetComponentsInChildren<ITankInputControllerHandler>(true);
            foreach (var handler in inputControllerHandlers)
            {
                if (handler != this)
                {
                    handler.BindInputController(tankInputController);
                }
            }
        }
    }
}