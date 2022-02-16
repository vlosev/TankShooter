using System;
using UnityEngine;

namespace Common.MathUtils
{
    public class MathUtils
    {
        public static float ClampAngle(float angle, float minAngle, float maxAngle)
        {
            if (angle > 180f)
            {
                while (angle > 180f)
                    angle -= 360f;
            }
            else if (angle < -180f)
            {
                while (angle > 180f)
                    angle += 360f;
            }

            return Mathf.Clamp(angle, minAngle, maxAngle);
        }
    }
}