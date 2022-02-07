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
    
    public class Gun : TankSingleShotWeapon
    {
        #region fsm
        private class GunState : FsmState<Gun>
        {
            protected GunState(Gun entity) : base(entity) { }
        }

        private class GunInit : GunState
        {
            public GunInit(Gun entity) : base(entity)
            {
                Debug.Log("enter state init");
            }

            public override void OnEnter()
            {
                Entity.state.Value = WeaponState.NotAvailable;
            }

            public override FsmState<Gun> Update()
            {
                return new GunIdle(Entity);
            }
        }
        
        private class GunIdle : GunState
        {
            public GunIdle(Gun entity) : base(entity)
            {
                Debug.Log("enter state idle");
            }

            public override void OnEnter()
            {
                Entity.state.Value = WeaponState.Idle;
            }

            public override FsmState<Gun> Update()
            {
                if (Entity.isShot)
                {
                    Entity.isShot = false;
                    return new GunShot(Entity);
                }
                
                return base.Update();
            }
        }
        
        private class GunShot : GunState
        {
            private float prepareToShotTime;
            
            public GunShot(Gun entity) : base(entity)
            {
                Debug.Log("enter state shot");
            }

            public override void OnEnter()
            {
                Entity.state.Value = WeaponState.Shot;
                prepareToShotTime = 0f;
            }

            public override FsmState<Gun> Update()
            {
                //ждем какое-то время
                prepareToShotTime += Time.deltaTime;
                if (prepareToShotTime < Entity.shotTime)
                {
                    return this;
                }
                
                Entity.PlayShotEffect();
                Entity.PlayShotSound();
                return new GunReload(Entity);
            }
        }
        
        private class GunReload : GunState
        {
            private float reloadTime;
            
            public GunReload(Gun entity) : base(entity)
            {
            }

            public override FsmState<Gun> Update()
            {
                reloadTime += Time.deltaTime;
                if (reloadTime >= Entity.reloadTime)
                {
                    return new GunIdle(Entity);
                }

                var t = Mathf.Clamp01(reloadTime / Entity.reloadTime);
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
        private Fsm<Gun> fsm;

        public override void Init(TankWeaponManager tankWeaponManager)
        {
            base.Init(tankWeaponManager);
            
            fsm = new Fsm<Gun>(new GunInit(this));
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