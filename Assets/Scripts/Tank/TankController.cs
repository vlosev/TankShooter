using TankShooter.Common;
using TankShooter.Game.Weapon;
using TankShooter.GameInput;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Tank
{
    public class TankController : NotifiableMonoBehaviour,
        ITankInputControllerHandler,
        ITank
    {
        [Header("Tank refrerences for constructor")] 
        [SerializeField] private Transform turretPivot;
        [SerializeField] private Transform chassisPivot;
        
        [Header("references")]
        [SerializeField] private Rigidbody tankRigidbody;
        [SerializeField] private TankWeaponManager weaponManager;

        private ITankInputController inputController;
        private TankContext tankContext;

        #region ITank implementation

        public TankContext Context => tankContext;
        Transform ITank.Transform => transform;
        Rigidbody ITank.Rigidbody => tankRigidbody;
        ITankInputController ITank.InputController => inputController;
        #endregion

        public TankWeaponManager WeaponManager => weaponManager;

        //сюда передаем какие-нибудь настройки танка
        public void Init(TankContext tankContext)
        {
            this.tankContext = tankContext;
            
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