namespace TankShooter.Game.Enemy
{
    /// <summary>
    /// пассивно агрессивная модель:
    /// эта модель агрится только в случае угрозы, то есть при приближении к врагу или нанесении урона врагом
    /// </summary>
    public class EnemyAIPassiveAggresive : EnemyAI
    {
        protected override EnemyAIState GetInitialAIState()
        {
            throw new System.NotImplementedException();
        }
    }
}