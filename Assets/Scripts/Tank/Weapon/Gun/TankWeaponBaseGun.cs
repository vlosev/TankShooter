using System;
using Tank.Weapon;
using TankShooter.Common.FSM;
using TankShooter.Game.Enemy;
using TankShooter.Game.Weapon;
using UnityEngine;

namespace TankShooter.Battle.TankCode
{
    public class GunSettings
    {
        public float Damage = 500;
        public float ReloadTime = 2;
    }
    
    public class TankWeaponBaseGun : TankSingleShotWeaponBase
    {
        #region fsm
        private class GunState : FsmState<TankWeaponBaseGun>
        {
            protected GunState(TankWeaponBaseGun entity) : base(entity) { }
        }

        private class GunInit : GunState
        {
            public GunInit(TankWeaponBaseGun entity) : base(entity)
            {
                Debug.Log("enter state init");
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.NotAvailable;
            }

            public override FsmState<TankWeaponBaseGun> Update()
            {
                return new GunIdle(entity);
            }
        }
        
        private class GunIdle : GunState
        {
            public GunIdle(TankWeaponBaseGun entity) : base(entity)
            {
                Debug.Log("enter state idle");
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Idle;
            }

            public override FsmState<TankWeaponBaseGun> Update()
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
            private float prepareToShotTime;
            
            public GunShot(TankWeaponBaseGun entity) : base(entity)
            {
                Debug.Log("enter state shot");
            }

            public override void OnEnter()
            {
                entity.state.Value = WeaponState.Shot;
                prepareToShotTime = 0f;
            }

            public override FsmState<TankWeaponBaseGun> Update()
            {
                //ждем какое-то время
                prepareToShotTime += Time.deltaTime;
                if (prepareToShotTime < entity.shotTime)
                {
                    return this;
                }
                
                entity.PlayShotEffect();
                entity.PlayShotSound();
                return new GunReload(entity);
            }
        }
        
        private class GunReload : GunState
        {
            private float reloadTime;
            
            public GunReload(TankWeaponBaseGun entity) : base(entity)
            {
            }

            public override FsmState<TankWeaponBaseGun> Update()
            {
                reloadTime += Time.deltaTime;
                if (reloadTime >= entity.reloadTime)
                {
                    return new GunIdle(entity);
                }

                var t = Mathf.Clamp01(reloadTime / entity.reloadTime);
                Debug.Log($"Reloading progress = {t}");
                return this;
            }
        }
        #endregion
        
        [SerializeField] private Transform shotPivot;
        [SerializeField] private AudioSource shotAudioSource;
        [SerializeField] private ParticleSystem shotEffect;

        private float shotTime = 0.5f;
        private float reloadTime = 5;
        private int availableShots = 1;
        private Fsm<TankWeaponBaseGun> fsm;

        public override TankWeaponSlotName SlotName => TankWeaponSlotName.Gun;

        public override void Init(TankWeaponManager tankWeaponManager)
        {
            base.Init(tankWeaponManager);
            
            fsm = new Fsm<TankWeaponBaseGun>(new GunInit(this));
        }

        private void Update()
        {
            fsm?.Update();
        }

        private void PlayShotSound()
        {
            shotAudioSource.Play();
        }

        private void PlayShotEffect()
        {
            Debug.Log($"Gun play shot effect");
        }
    }
}