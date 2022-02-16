using System;
using TankShooter.Common.FSM;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Tank.Weapon.MachineGun
{
    public class TankWeaponMachineGun : TankAutomaticWeaponBase
    {
        [Serializable]
        private class Cannon
        {
            [SerializeField] private Transform shotPositionTransform;
            
            public Transform GetShotPositionTransform()
            {
                return shotPositionTransform;
            }
        }

        [Tooltip("время подготовки к выстрелу")] 
        [SerializeField] private float prepareShootingTime = 1f;
        [Tooltip("время остановки пулемета")] 
        [SerializeField] private float stoppingShootingTime = 3f;
        [Tooltip("скорострельность в выстрелов/минуту")] 
        [SerializeField] private int shotsPerSecond = 120;
        [Tooltip("время перезарядки пулемета")]
        [SerializeField] private float reloadingTime = 1f;
        [Tooltip("скорость вращения стволов в градусах в секунду")]
        [SerializeField] private float rotationSpeed = 30f;
        [Tooltip("трансформ диска для вращения стволов")]
        [SerializeField] private Transform rotator;
        [Tooltip("позиции кончиков стволов для проигрывания эффекта выстрела")]
        [SerializeField] private Cannon[] cannons;
        [Tooltip("кол-во патронов доступных для выстрелов")]
        [SerializeField] private uint bulletsAvailableCount;
        [Tooltip("префаб пули")] 
        [SerializeField] private MachineGunBullet bulletPrefab;
        [Tooltip("звуки вращения стволов")]
        [SerializeField] private AudioSource rotateSoundSource;
        [SerializeField] private AudioClip shootingBeginClip;
        [SerializeField] private AudioClip shootingClip;
        [SerializeField] private AudioClip shootingEndClip;

        private float shotsDelay = 0f;
        private uint bulletsRemainCount;
        private Fsm<TankWeaponMachineGun> fsm;

        public override TankWeaponSlotName SlotName => TankWeaponSlotName.MachineGun;

        public override void Init(TankWeaponManager weaponManager, TankWeaponSlot weaponSlot)
        {
            base.Init(weaponManager, weaponSlot);

            fsm = new Fsm<TankWeaponMachineGun>(new GunInit(this));
        }

        private void Update()
        {
            fsm?.Update();
        }

        private void RotateCannons(float angleDelta)
        {
            var eulerAngles = rotator.localEulerAngles;
            eulerAngles.z = Mathf.Repeat(Time.deltaTime * angleDelta + eulerAngles.z, 360);
            rotator.localEulerAngles = eulerAngles;
        }

        #region fsm
        private class GunState : FsmState<TankWeaponMachineGun>
        {
            protected readonly AudioSource audioSource;
            
            protected GunState(TankWeaponMachineGun entity) : base(entity)
            {
                Debug.Log($"[{entity.GetType().Name}] create state '{GetType().Name}'");

                audioSource = entity.rotateSoundSource;
            }
        }
        
        private class GunInit : GunState
        {
            public GunInit(TankWeaponMachineGun entity) : base(entity)
            {
                //TODO: если сериализуем стейт, здесь надо поднять кол-во патронов из стейта, которое осталось 
                entity.bulletsRemainCount = entity.bulletsAvailableCount;
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.NotAvailable;
            }

            public override FsmState<TankWeaponMachineGun> Update()
            {
                return new GunIdle(entity);
            }
        }
        
        private class GunIdle : GunState
        {
            public GunIdle(TankWeaponMachineGun entity) : base(entity)
            {
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Idle;
            }

            public override FsmState<TankWeaponMachineGun> Update()
            {
                if (entity.isShooting)
                    return new GunPrepareShooting(entity, 0f);
                return base.Update();
            }
        }
        
        private class GunPrepareShooting : GunState
        {
            private float lastTime;
            private float currentRotationSpeed;
            
            public GunPrepareShooting(TankWeaponMachineGun entity, float currentRotationSpeed) : base(entity)
            {
                this.currentRotationSpeed = currentRotationSpeed;
                this.lastTime = Mathf.Clamp01(1f - currentRotationSpeed / entity.rotationSpeed) * entity.prepareShootingTime;
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Shot;
                
                audioSource.volume = 0;
                audioSource.loop = false;
                audioSource.PlayOneShot(entity.shootingBeginClip);

                base.OnEnter();
            }

            public override FsmState<TankWeaponMachineGun> Update()
            {
                //если закончились патроны, попробуем перезарядить оружие
                //если передумали стрелять, но пулемет уже разогнался, даем ему остановиться корректно
                if (entity.bulletsRemainCount == 0 || !entity.isShooting && currentRotationSpeed > 0f)
                    return new GunStoppingShooting(entity, currentRotationSpeed);
                
                //если вышло время для разгона - переходим в состояние стрельбы
                if (lastTime <= 0f)
                    return new GunShot(entity);

                var dt = Time.deltaTime;
                
                //раскручиваем пулемет
                currentRotationSpeed = Mathf.Clamp01(1f - lastTime) * entity.rotationSpeed;
                entity.RotateCannons(currentRotationSpeed);

                lastTime -= dt - dt * dt;

                entity.rotateSoundSource.volume = Mathf.Clamp01(1 - lastTime);
                return base.Update();
            }
        }
        
        private class GunShot : GunState
        {
            public GunShot(TankWeaponMachineGun entity) : base(entity)
            {
            }

            public override void OnEnter()
            {
                audioSource.clip = entity.shootingClip;
                audioSource.loop = true;
                audioSource.Play();
                
                base.OnEnter();
            }

            public override FsmState<TankWeaponMachineGun> Update()
            {
                if (entity.isShooting != true)
                    return new GunStoppingShooting(entity, entity.rotationSpeed);

                entity.RotateCannons(entity.rotationSpeed);
                return base.Update();
            }

            private void DoShot()
            {
                //получаем снаряд из менеджера, он будет создан из пулла, чтобы не пересозадвать постоянно
                //создаем контекст, чтобы передать туда колбэк на релиз в пул и параметры оружия, для начисления очкой и т.д.
                var projectile = entity.ProjectileManager.GetProjectile(entity.bulletPrefab);
                if (projectile is IProjectile<TankWeaponMachineGun, MachineGunBulletContext> projectileContextHolder)
                {
                    var ctx = new MachineGunBulletContext(entity, () => entity.ProjectileManager.ReleaseProjectile(projectile));
                    projectileContextHolder.Init(ctx);
                }

                //TODO: сделать отсчет стволов в обратном порядке и спавнить пулю из разных
                var cannon = entity.cannons[0];
                var shotPositionTransform = cannon.GetShotPositionTransform();
                
                var projectileTransform = projectile.transform;
                projectileTransform.position = shotPositionTransform.position;
                projectileTransform.forward = shotPositionTransform.forward;
            }
        }
        
        private class GunStoppingShooting : GunState
        {
            private float lastTime;
            private float rotationSpeed;
            
            public GunStoppingShooting(TankWeaponMachineGun entity, float rotationSpeed) : base(entity)
            {
                this.rotationSpeed = rotationSpeed;
                this.lastTime = Mathf.Clamp01(rotationSpeed / entity.rotationSpeed) * entity.stoppingShootingTime;
            }

            public override void OnEnter()
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(entity.shootingEndClip);
                base.OnEnter();
            }

            public override FsmState<TankWeaponMachineGun> Update()
            {
                if (entity.isShooting)
                    return new GunPrepareShooting(entity, rotationSpeed);
                
                if (lastTime <= 0f)
                    return new GunIdle(entity);
                
                var dt = Time.deltaTime;
                lastTime = Mathf.Clamp01(lastTime - dt - dt * dt);
                rotationSpeed = entity.rotationSpeed * lastTime;
                
                entity.RotateCannons(rotationSpeed);
                audioSource.volume = lastTime;
                
                return base.Update();
            }
        }
        
        private class GunReload : GunState
        {
            private float reloadTime;
            
            public GunReload(TankWeaponMachineGun entity) : base(entity)
            {
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Reload;
                base.OnEnter();
            }
            
            public override FsmState<TankWeaponMachineGun> Update()
            {
                if (reloadTime >= entity.reloadingTime)
                {
                    entity.bulletsRemainCount = entity.bulletsAvailableCount;
                    return new GunIdle(entity);
                }

                reloadTime += Time.deltaTime;
                return base.Update();
            }
        }
        #endregion
    }
}