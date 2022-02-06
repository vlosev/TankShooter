using Tank.Interfaces;
using TankShooter.Battle;
using TankShooter.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankShooter.Battle
{
    public class BattleContext : MonoBehaviour
    {
        [SerializeField] private Tank playerTank;

        public void StartBattle(LevelContext levelContext)
        {
            playerTank.BindInputController(levelContext.PlayerInputController);
        }
    }
}