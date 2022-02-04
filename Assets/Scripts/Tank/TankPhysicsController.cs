using System;
using TankShooter.Common;
using UnityEngine;

namespace TankShooter.Battle
{
    public class TankPhysicsController : NotifiableMonoBehaviour, IPhysicsBeforeTickListener
    {
        //колеса танка
        [SerializeField] private WheelCollider[] LWheels;
        [SerializeField] private WheelCollider[] RWheels;
        
        //некоторые настройки физики
        
        protected override void SafeAwake()
        {
            BattleTimeMachine.SubscribePhysicsBeforeTick(this).SubscribeToDispose(this);
        }

        public void OnBeforePhysicsTick(float dt)
        {
        }
    }
}