using Common;
using TankShooter.Battle;
using TankShooter.Game.Enemy;
using TankShooter.Game.HUD;
using TankShooter.Game.Weapon;
using TankShooter.GameInput;
using TankShooter.Tank;
using TankShooter.Tank.Constructor;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Game
{
    /// <summary>
    /// контекст уровня, где хранятся ссылки на все необходимое бою
    /// </summary>
    public class LevelContext : MonoBehaviour
    {
        [SerializeField] private TankConstructor tankConstructor;
        [SerializeField] private InputController inputController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private ProjectileManager projectileManager;
        
        [SerializeField] private TankUI tankUI;
        
        private void Start()
        {
            cameraController.BindInputController(inputController);
            InitEnemyManager();
            CreatePlayerTank();
        }

        private void InitEnemyManager()
        {
            enemyManager.Init(this);
        }

        private void CreatePlayerTank()
        {
            var weaponManagerCtx = new TankWeaponManagerContext(projectileManager);
            var playerTankCtx = new TankContext(weaponManagerCtx);
            
            var playerTank = tankConstructor.CreateTank(
                playerSpawnPoint,
                0, //body
                0, //chassis
                1, //turret
                new[]
                {
                    (TankWeaponSlotName.Gun, 0),
                    (TankWeaponSlotName.MachineGun, 0)
                });
            
            if (playerTank.TryGetComponent<TankController>(out var tank))
            {
                tank.BindInputController(inputController);
                tank.Init(playerTankCtx);
                
                playerTank.gameObject.SetLayerRecursively(LayerMask.NameToLayer("PlayerTank"));
                tankUI.BindTankController(tank);
            }
            
            cameraController.SetTarget(playerTank.transform);
        }
    }
}