using System.Security.Principal;
using TankShooter.Battle;

namespace Tank.Interfaces
{
    /// <summary>
    /// интерфейс модуля танка необходим для того, чтобы танк мог инициализировать все необходимое нужными данными
    /// </summary>
    public interface ITankModule
    {
        void Init(ITank tank);
    }
}