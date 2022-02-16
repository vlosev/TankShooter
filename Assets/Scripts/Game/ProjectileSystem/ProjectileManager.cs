using System;
using System.Collections.Generic;
using Common;
using TankShooter.Common;
using TankShooter.Tank.Weapon;
using UnityEngine;

namespace TankShooter.Game.Weapon
{
    //вместо системы в ECS пусть это будет просто менеджер, который двигает пули, следит за ними и в конце кадра
    //удалит тех, кто уже отработал или кому уже не надо
    public class ProjectileManager : NotifiableMonoBehaviour, IPhysicsBeforeTickListener
    {
        [Serializable]
        private struct ProjectileSettings
        {
            [SerializeField] private Projectile prefab;
            public Projectile Prefab => prefab;

            [SerializeField] private int prefabsCount;
            public int PrefabsCount => prefabsCount;

            [SerializeField] private Transform poolContainer;
            public Transform PoolContainer => poolContainer;
        }

        [SerializeField] private ProjectileSettings[] projectileSettings;

        //пуль пуль/снарядов/ракет
        private readonly ObjectPool<Projectile> projectliesPool = new ObjectPool<Projectile>();
        
        //активные пули, которые в данный момент летят к своей цели
        private readonly List<Projectile> activeProjectiles = new List<Projectile>();
        
        //пули, которые в конце кадра нужно будет удалить
        private readonly List<Projectile> removeProjectiles = new List<Projectile>();

        protected override void SafeAwake()
        {
            base.SafeAwake();
            
            foreach (var ps in projectileSettings)
            {
                var container = ps.PoolContainer != null ? ps.PoolContainer : transform;
                projectliesPool.RegisterObject(container, ps.Prefab, ps.PrefabsCount);
            }
        }

        //вначале кадра обновляем логику всех снарядов, их время жизни и т.д.
        private void Update()
        {
            //сначала обновляем все снаряды
            var dt = Time.deltaTime;
            for (int i = 0; i < activeProjectiles.Count; ++i)
            {
                activeProjectiles[i].UpdateVisual(dt);
            }
        }

        //здесь апдейт сделан таким образом, чтобы можно было управлять скоростью и движением пуль
        //из одной точки и чтобы не было много monobehaviour с update/lateupdate/fixedupdate
        //1) апдейты в каждом монобехе - медленно
        //2) профалить проще, когда они все в одной точке, ну или их мало хотя бы
        void IPhysicsBeforeTickListener.OnBeforePhysicsTick(float dt)
        {
            for (int i = 0; i < activeProjectiles.Count; ++i)
            {
                activeProjectiles[i].UpdatePhysics(dt);
            }
        }

        //в конце кадра проходим по тем, кого по разным причинам нужно уничтожить и собрать в кучу
        private void LateUpdate()
        {
            if (removeProjectiles.Count > 0)
            {
                for (int i = 0; i < removeProjectiles.Count; ++i)
                {
                    var projectile = removeProjectiles[i];
                    if (activeProjectiles.Remove(projectile))
                    {
                        projectliesPool.Release(projectile);
                    }
                }
                
                removeProjectiles.Clear();
            }
        }

        public Projectile GetProjectile(Projectile projectilePrefab)
        {
            if (projectilePrefab == null)
                throw new Exception("Can't instantiate null projectile prefab!");

            var projectile = projectliesPool.InstantiateFromPool(projectilePrefab);
            projectile.gameObject.SetActive(true);
            
            activeProjectiles.Add(projectile);
            
            return projectile;
        }

        public void ReleaseProjectile(Projectile projectile)
        {
            if (removeProjectiles.Contains(projectile) != true)
            {
                removeProjectiles.Add(projectile);
            }
        }
    }
}