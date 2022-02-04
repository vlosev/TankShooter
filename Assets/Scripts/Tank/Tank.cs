using System;
using TankShooter.Battle.TankCode;
using TankShooter.Common;
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
    
    public class Tank : MonoBehaviour,
        ITank
    {
        private IInputController inputController;

        [SerializeField] private Rigidbody tankRigidbody;
        [SerializeField] private Transform centerOfMass;

        [SerializeField] private TankTrack rightTrack;

        #region ITank implementation
        Rigidbody ITank.Rigidbody => tankRigidbody;
        
        IInputController ITank.InputContoller => inputController;
        #endregion

        //TODO: сделать загрузку и настройку
        [SerializeField]
        private TankSettings settings = new TankSettings()
        {
            MaxSpeed = 2000,
            MaxTurnSpeed = 3
        };

        private void Awake()
        {
            tankRigidbody.centerOfMass = centerOfMass.localPosition;
        }

        public void Setup(IInputController inputController)
        {
            this.inputController = inputController;

            var modules = GetComponentsInChildren<ITankModule>(true);
            foreach (var module in modules)
            {
                module.SetupModule(this);
            }
        }
    }
}