using System;
using Tank.Interfaces;
using Tank.Weapon;
using TankShooter.Battle.TankCode;
using TankShooter.Common;
using TankShooter.Game;
using TankShooter.Game.Enemy;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankShooter.Battle
{
    [Serializable]
    public class TankSettings
    {
        public float MaxSpeed = 500f;
        public float MaxTurnSpeed = 5f;
    }
    
    public class Tank : NotifiableMonoBehaviour,
        ITankInputControllerHandler,
        ITank
    {
        private ITankInputController inputController;
        private ITankModule[] tankModules;

        [Header("references")]
        [SerializeField] private Rigidbody tankRigidbody;
        [SerializeField] private TankWeaponManager weaponManager;
        
        [Header("gameplay parameters")]
        [SerializeField] private Transform centerOfMass;

        #region ITank implementation
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
            tankModules = GetComponentsInChildren<ITankModule>(true);
            foreach (var module in tankModules)
            {
                module.Init(this);
            }
        }

        public void BindInputController(ITankInputController tankInputController)
        {
            inputController = tankInputController;
            foreach (var module in tankModules)
            {
                if (module is ITankInputControllerHandler inputControllerHandler)
                    inputControllerHandler.BindInputController(tankInputController);
            }
        }
    }
}