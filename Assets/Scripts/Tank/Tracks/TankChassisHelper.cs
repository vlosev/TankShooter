using UnityEngine;

namespace TankShooter.Tank
{
    public partial class TankChassis
    {
        private void SetSidewaysStiffness(WheelCollider wheelCollider, float value)
        {
            var frictionCurve = wheelCollider.sidewaysFriction;
            frictionCurve.stiffness = value;
            wheelCollider.sidewaysFriction = frictionCurve;
        }

        private void SetForwardStiffness(WheelCollider wheelCollider, float value)
        {
            var frictionCurve = wheelCollider.forwardFriction;
            frictionCurve.stiffness = value;
            wheelCollider.forwardFriction = frictionCurve;
        }
    }
}