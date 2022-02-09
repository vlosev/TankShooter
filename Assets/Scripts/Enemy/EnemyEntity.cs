using System;
using TankShooter.Common;
using TankShooter.Game.Enemy;
using UnityEngine;
using Common;

namespace TankShooter.Game.Enemy
{
    public class EnemyContext
    {
        public readonly LevelContext LevelContext;
        public readonly EnemyPathManager PathManager;

        public EnemyContext(LevelContext levelContext, EnemyPathManager pathManager)
        {
            LevelContext = levelContext;
            PathManager = pathManager;
        }
    }
    
    public class EnemyEntity : NotifiableMonoBehaviour
    {
        public EnemyContext Context { get; private set; }
        
        // private Enem enemyAi;
        // private IHealthEntity healthEntity;
        // private Animator animator;
        //
        public event Action<EnemyEntity> OnStartEnemy;
        
        public event Action<EnemyEntity> OnDeadEnemy;
        //
        // public EnemyAI AI => enemyAi;
        //
        // protected override void SafeAwake()
        // {
        //     base.SafeAwake();
        //     if (TryGetComponent<IHealthEntity>(out var healthEntity))
        //     {
        //         healthEntity.IsAlive.SubscribeChanged(OnIsAliveChanged).SubscribeToDispose(this);
        //         this.healthEntity = healthEntity;
        //     }
        //     
        //     enemyAi = GetComponent<EnemyAI>();
        //     if (this.TryGetComponentInChildren(out EnemyTrigger trigger))
        //         trigger.SetEnemyEntity(this);
        // }

        public void StartEnemy(EnemyContext context)
        {
            //reset hp
            
            Context = context;
            OnStartEnemy?.Invoke(this);
        }

        private void OnIsAliveChanged(bool isAlive)
        {
            if (isAlive != true)
            {
                OnDeadEnemy?.Invoke(this);
            }
        }
    }
}