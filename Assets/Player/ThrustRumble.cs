using UniDi;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class ThrustRumble : MonoBehaviour
    {
        private ThrustInfo _thrustInfo;
        public float maxDirectionalThrust = 500_000;
        public float maxRotationalThrust = 600_000;
        public float thrustDirectioalScale = 0.5f;
        public float thrustRotationalScale = 0.3f;

        public void Update()
        {
            var absoluteDirectionalThrust = Mathf.Abs((_thrustInfo.CurrentDirectionalThrust / maxDirectionalThrust) * thrustDirectioalScale);
            var absoluteRotationalThrust = Mathf.Abs((_thrustInfo.CurrentRotationalThrust / maxRotationalThrust) * thrustRotationalScale);
            var thrust = Mathf.Max(absoluteDirectionalThrust, absoluteRotationalThrust);
            GetGamepad()?.SetMotorSpeeds(thrust, 0);
        }

        [Inject]
        public void Init(ThrustInfo thrustInfo)
        {
            _thrustInfo = thrustInfo;
        }
        
        private Gamepad GetGamepad() => Gamepad.current;
        
    }
}