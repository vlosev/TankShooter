using TankShooter.Common;

namespace TankShooter.Game
{
    public interface IHealthEntity
    {
        //здоровье
        IReadonlyReactiveProperty<float> Health { get; }
        
        //жив ли объект
        IReadonlyReactiveProperty<bool> IsAlive { get; }
    }
}