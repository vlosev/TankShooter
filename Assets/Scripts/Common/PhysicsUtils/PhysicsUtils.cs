namespace Common.PhysicsUtils
{
    public static class PhysicsUtils
    {
        public static float ConvertSpeedMStoKMH(float speedMs)
        {
            return speedMs * 3.6f;
        }
    }
}