namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// пассивная модель:
    /// она не агрится вообще, просто идет от точки A к точке B и выбирает следующую
    /// если врезался во врага - нанес урон, целенаправленно не делает этого 
    /// </summary>
    public class EnemyAIPassive : EnemyAI
    {
        protected override EnemyAIState GetInitialAIState()
        {
            throw new System.NotImplementedException();
        }
    }
}