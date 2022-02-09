using TankShooter.Battle;
using TankShooter.Battle.Tank;
using TankShooter.Battle.TankCode;
using TankShooter.Game.Enemy;
using TankShooter.GameInput;
using TankShooter.Tank;
using TankShooter.Tank.Constructor;
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
            var playerTank = tankConstructor.CreateTank(
                playerSpawnPoint,
                0, //body
                0, //chassis
                1, //turret
                new[]
                {
                    (TankWeaponSlotName.Gun, 0)
                });

            if (playerTank.TryGetComponent<TankController>(out var tank))
            {
                tank.BindInputController(inputController);
                tank.InitTank(/* TODO add player settings */);
            }
            
            cameraController.SetTarget(playerTank.transform);
        }
    }
}