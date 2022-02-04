using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankShooter.Game
{
    public interface IDamageReceiver
    {
        void OnDamage(float damage);
    }
}