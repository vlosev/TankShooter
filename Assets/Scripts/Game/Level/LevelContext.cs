using System;
using System.Collections;
using System.Collections.Generic;
using TankShooter.Battle;
using UnityEngine;

namespace TankShooter.Game
{
    /// <summary>
    /// контекст уровня, где хранятся ссылки на все необходимое бою
    /// </summary>
    public class LevelContext : MonoBehaviour
    {
        [SerializeField] private InputController inputController;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private BattleContext battleContext;

        public ITankInputController PlayerInputController => inputController;
        
        public readonly DamageSystem sysDamage = new DamageSystem();
        public readonly ProjectileSystem sysProjectile = new ProjectileSystem();

        private void Start()
        {
            cameraController.BindInputController(inputController);
            battleContext.StartBattle(this);
        }

        private void Update()
        {
            var dt = Time.deltaTime;
            
            sysDamage.Update(dt);
            sysProjectile.Update(dt);
        }
    }
}