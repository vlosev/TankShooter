using TankShooter.Battle;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankShooter
{
    public class BattleContext : MonoBehaviour
    {
        [SerializeField] private Tank playerTank;
        [SerializeField] private InputControllerKeyboardAndMouse playerInput;
        [SerializeField] private CameraController cameraController;

        private void Start()
        {
            playerTank.Setup(playerInput);
            cameraController.SetInputController(playerInput);
        }
    }
}