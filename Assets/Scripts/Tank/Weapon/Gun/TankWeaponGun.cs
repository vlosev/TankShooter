using TankShooter.Common.FSM;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Weapon.Gun
{
    public class GunSettings
    {
        public float Damage = 500;
        public float ReloadTime = 2;
    }
    
    public class TankWeaponGun : TankSingleShotWeaponBase
    {
        [SerializeField] private GunProjectile projectilePrefab = null;
        [SerializeField] private float reloadTime = 0.2f;
        [SerializeField] private float shotDelay = 0.05f;
        [SerializeField] private float projectlieLifeTime = 3f;

        [SerializeField] private Transform shotPivot;
        [SerializeField] private AudioSource shotAudioSource;
        [SerializeField] private ParticleSystem shotEffect;

        private Fsm<TankWeaponGun> fsm;

        public override TankWeaponSlotName SlotName => TankWeaponSlotName.Gun;

        public override void Init(TankWeaponManager weaponManager, TankWeaponSlot weaponSlot)
        {
            base.Init(weaponManager, weaponSlot);
            
            fsm = new Fsm<TankWeaponGun>(new GunInit(this));
        }

        private void Update()
        {
            fsm?.Update();
        }

        private void PlayShotSound()
        {
            if (shotAudioSource != null)
            {
                shotAudioSource.Play();
            }
        }

        private void PlayShotEffect()
        {
            if (shotEffect != null)
            {
                shotEffect.Play(true);
            }
        }
        
        #region fsm
        private class GunState : FsmState<TankWeaponGun>
        {
            protected GunState(TankWeaponGun entity) : base(entity) { }
        }

        private class GunInit : GunState
        {
            public GunInit(TankWeaponGun entity) : base(entity)
            {
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.NotAvailable;
            }

            public override FsmState<TankWeaponGun> Update()
            {
                return new GunIdle(entity);
            }
        }
        
        private class GunIdle : GunState
        {
            public GunIdle(TankWeaponGun entity) : base(entity)
            {
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Idle;
                entity.isShot = false;
            }

            public override FsmState<TankWeaponGun> Update()
            {
                if (entity.isShot)
                {
                    entity.isShot = false;
                    return new GunShot(entity);
                }
                
                return base.Update();
            }
        }
        
        private class GunShot : GunState
        {
            private readonly Projectile projectilePrefab;
            private readonly ProjectileManager projectileManager;
            private float prepareToShotTime;
            
            public GunShot(TankWeaponGun entity) : base(entity)
            {
                projectilePrefab = entity.projectilePrefab;
                projectileManager = entity.ProjectileManager;
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Shot;
                prepareToShotTime = 0f;
            }

            public override FsmState<TankWeaponGun> Update()
            {
                //ждем какое-то время
                prepareToShotTime += Time.deltaTime;
                if (prepareToShotTime < entity.shotDelay)
                {
                    return this;
                }

                DoShot();
                return new GunReload(entity);
            }

            private void DoShot()
            {
                var projectile = projectileManager.GetProjectile(projectilePrefab);
                var contextHolder = projectile as IProjectile<TankWeaponGun, GunProjectileContext>;  
                if (contextHolder != null)
                {
                    var ctx = new GunProjectileContext(
                        entity,
                        () => projectileManager.ReleaseProjectile(projectile));
                    
                    contextHolder.Init(ctx);
                    projectile.gameObject.SetActive(true);
                }
                
                var shotTransform = entity.shotPivot;
                
                var projectileTransform = projectile.transform;
                projectileTransform.position = shotTransform.position;
                projectileTransform.forward = shotTransform.forward;
                
                entity.PlayShotEffect();
                entity.PlayShotSound();
            }
        }
        
        private class GunReload : GunState
        {
            private float reloadTime;
            
            public GunReload(TankWeaponGun entity) : base(entity)
            {
            }

            public override FsmState<TankWeaponGun> Update()
            {
                reloadTime += Time.deltaTime;
                if (reloadTime >= entity.reloadTime)
                {
                    return new GunIdle(entity);
                }

                var t = Mathf.Clamp01(reloadTime / entity.reloadTime);
                return this;
            }
        }
        #endregion
    }
}