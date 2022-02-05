using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TankShooter.Game
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [Serializable]
        private struct EnemySpawnDescription
        {
            [SerializeField] private EnemyEntity prefab;
            [SerializeField] private float chance;

            public EnemyEntity Prefab => prefab;
            public float Chance => chance;
        }

        [SerializeField] private EnemySpawnDescription[] spawnPrefabs;

        public Transform SpawnPointTransform => transform;

        public EnemyEntity GetPrefab()
        {
            //TODO: compute drop with probability
            //var rnd = Random.Range(0f, 1f);
            //for (int i = 0; i < )

            return spawnPrefabs[0].Prefab;
        }

        //массив префабов отсюда нужен только для формирования пула вначале, далее мы из точки будем получать только префаб
        public EnemyEntity[] GetPrefabs()
        {
            return spawnPrefabs.Select(p => p.Prefab).ToArray();
        }
    }
}