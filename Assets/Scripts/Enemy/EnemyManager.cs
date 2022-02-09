using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// менеджер врагов следит за тем, нужно ли вообще спавнить, например, врагов стало меньше 10
    /// ищет ближайшую к игроку точку, запрашивает у нее префаб - кого заспавнить и делает это 
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private int maxEnemiesOnLevel = 10;
        [SerializeField] private float minSpawnTime = 2f;
        [SerializeField] private float maxSpawnTime = 10f;

        [SerializeField] private EnemySpawnPoint[] enemySpawnPoints;

        private LevelContext levelContext;
        private int needToSpawnEnemiesCount;
        private float timeToSpawn = 0f;
        private readonly List<EnemyEntity> enemies = new List<EnemyEntity>();
        private readonly ObjectPool<EnemyEntity> enemiesPool = new ObjectPool<EnemyEntity>();

        public void Init(LevelContext levelContext)
        {
            this.levelContext = levelContext;

            //заполняем пул объектами, которые, возможно, придется создавать
            var prefabs = enemySpawnPoints.SelectMany(i => i.GetPrefabs()).Distinct();
            foreach (var prefab in prefabs)
            {
                enemiesPool.RegisterObject(transform, prefab, maxEnemiesOnLevel);
            }
            
            //спавним все 
            SpawnEnemiesOnStart();
        }

        private void LateUpdate()
        {
            //каждый кадр смотрим, нужно ли кого-то заспавнить
            if (needToSpawnEnemiesCount > 0)
            {
                //если нужно, считаем сколько нужно подождать и по истечении времени спавним и декрементим счетчик
                timeToSpawn -= Time.deltaTime;

                if (timeToSpawn < 0)
                {
                    needToSpawnEnemiesCount--;
                }
            }
        }
        
        private void SpawnEnemiesOnStart()
        {
            for (int i = 0; i < maxEnemiesOnLevel; ++i)
            {
                var rnd = Random.Range(0, enemySpawnPoints.Length - 1);
                CreateEnemy(enemySpawnPoints[rnd]);
            }
        }
        
        private void CreateEnemy(EnemySpawnPoint spawnPoint)
        {
            var spawnData = spawnPoint.GetRandomSpawnData();

            var enemyInstance = enemiesPool.InstantiateFromPool(spawnData.EnemyPrefab);
            var enemyTransform = enemyInstance.transform;
            
            enemyTransform.SetParent(spawnPoint.SpawnPointTransform);
            enemyTransform.position = spawnPoint.SpawnPointTransform.position;
            enemyInstance.gameObject.SetActive(true);

            var enemyContext = new EnemyContext(levelContext, spawnData.PathManager);
            enemyInstance.StartEnemy(enemyContext);
        }

        private void OnDeadEnemy(EnemyEntity enemy)
        {
            if (enemies.Remove(enemy))
            {
                // enemy.OnDeadEnemy -= OnDeadEnemy;
                timeToSpawn = GetTimeToRespawn();

                needToSpawnEnemiesCount++;
            }
        }

        private float GetTimeToRespawn()
        {
            return Random.Range(minSpawnTime, maxSpawnTime);
        }
    }
}