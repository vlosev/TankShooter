namespace TankShooter.Battle
{
    /// <summary>
    /// состояние оружия, глобально их может быть несколько
    /// </summary>
    enum WeaponState
    {
        //пусто - не готово к стрельбе, например, нет патронов/снарядов
        None,
        //готово к стрельбе
        Ready,
        //в процессе выстрела
        Shot,
        //в процессе перезарядки
        Reload,
        //сломано
        Crashed,
    }
    
    /// <summary>
    /// type:
    /// оружие танка
    /// parameters:
    /// углы ограничения поворота относительно танка
    /// углы ограничения наклона/подъема
    /// время перезарядки
    /// кол-во выстрелов без перезарядки
    /// </summary>
    public abstract class TankWeapon
    {
        public abstract void Shot();
        
        public abstract void Reload();
    }
}