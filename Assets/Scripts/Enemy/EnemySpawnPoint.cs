using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankShooter.Game.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        public struct SpawnData
        {
            public readonly EnemyEntity EnemyPrefab;
            public readonly EnemyPathManager PathManager;

            public SpawnData(EnemyEntity prefab, EnemyPathManager pathManager)
            {
                EnemyPrefab = prefab;
                PathManager = pathManager;
            }
        }
        
        [Serializable]
        private struct EnemySpawnDescription
        {
            [SerializeField] private EnemyEntity prefab;
            [SerializeField] private float chance;
            [SerializeField] private EnemyPathManager[] pathManagers;

            public EnemyEntity Prefab => prefab;
            public float Chance => chance;
            public EnemyPathManager[] PathManagers => pathManagers;
        }

        [SerializeField] private EnemyPathManager defaultPathManager;
        [SerializeField] private EnemySpawnDescription[] spawnPrefabs;

        public Transform SpawnPointTransform => transform;

        public SpawnData GetRandomSpawnData()
        {
            //TODO: compute drop with probability
            var spawnDescription = spawnPrefabs[0];
            var pathManager = defaultPathManager;
            var pathManagers = spawnDescription.PathManagers;
            if (pathManagers != null && pathManagers.Length != 0)
            {
                pathManager = pathManagers[Random.Range(0, pathManagers.Length)];
            }

            return new SpawnData(spawnDescription.Prefab, pathManager);
        }

        //массив префабов отсюда нужен только для формирования пула вначале, далее мы из точки будем получать только префаб
        public EnemyEntity[] GetPrefabs()
        {
            return spawnPrefabs.Select(p => p.Prefab).ToArray();
        }
    }
}