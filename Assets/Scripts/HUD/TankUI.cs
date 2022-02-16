using Common.PhysicsUtils;
using TankShooter.Tank;
using UnityEngine;
using UnityEngine.UI;

namespace TankShooter.Game.HUD
{
    public class TankUI : MonoBehaviour
    {
        [SerializeField] private Text labelTankSpeed;

        private ITank tank;
        
        public void BindTankController(ITank tank)
        {
            this.tank = tank;
        }

        private void Update()
        {
            if (tank == null)
                return;

            var speedKmh = Mathf.Round(PhysicsUtils.ConvertSpeedMStoKMH(tank.Rigidbody.velocity.magnitude));
            labelTankSpeed.text = $"{speedKmh} km/h";
        }
    }
}