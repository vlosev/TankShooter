using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankShooter.Game
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

        private int needToSpawnEnemiesCount;
        private float timeToSpawn = 0f;
        private readonly List<EnemyEntity> enemies = new List<EnemyEntity>();
        private readonly ObjectPool<EnemyEntity> enemiesPool = new ObjectPool<EnemyEntity>();

        private void Start()
        {
            //заполняем пул объектами, которые, возможно, придется создавать
            var prefabs = enemySpawnPoints.SelectMany(i => i.GetPrefabs()).Distinct();
            foreach (var prefab in prefabs)
            {
                enemiesPool.RegisterObject(prefab, maxEnemiesOnLevel);
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
        }
        
        private void CreateEnemy(EnemyEntity prefab)
        {
            var inst = Instantiate(prefab);
        }

        private void OnDeadEnemy(EnemyEntity enemy)
        {
            if (enemies.Remove(enemy))
            {
                enemy.OnDeadEnemy -= OnDeadEnemy;
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