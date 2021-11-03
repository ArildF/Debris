using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerControl : MonoBehaviour
    {
        public InputAction thrustAction;
        public InputAction reverseThrustAction;
        public InputAction lateralRollAction;
        public InputAction medialRollAction;
        public InputAction lateralThrustAction;
        public InputAction verticalThrustAction;
    
        public float forwardForce = 1_000_000;
        public float reverseForce = 500_000;
        public float lateralRollForce = 100_000;
        public float medialRollForce = 100_000;
        public float lateralThrustForce = 100_000;
        public float verticalThrustForce = 100_000;
        private Rigidbody _rigidBody;
        private InputAction[] _actions;

        // Start is called before the first frame update
        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        

        }

        private void OnEnable()
        {
            _actions = new[]
            {
                thrustAction, reverseThrustAction, lateralRollAction, medialRollAction, lateralThrustAction,
                verticalThrustAction
            }; 
        
            foreach(var action in _actions)
                action.Enable();
        }
    
        private void OnDisable()
        {
            foreach(var action in _actions)
                action.Disable();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (thrustAction.phase == InputActionPhase.Started)
            {
                _rigidBody.AddForce(transform.forward * forwardForce, ForceMode.Impulse); 
            }
            if (reverseThrustAction.phase == InputActionPhase.Started)
            {
                _rigidBody.AddForce(transform.forward * -reverseForce, ForceMode.Impulse); 
            }

            if (lateralRollAction.phase == InputActionPhase.Started)
            {
                var roll = lateralRollAction.ReadValue<float>();
                _rigidBody.AddRelativeTorque(0, 0, -roll * lateralRollForce);
            }
        
            if (medialRollAction.phase == InputActionPhase.Started)
            {
                var roll = medialRollAction.ReadValue<float>();
                _rigidBody.AddRelativeTorque(roll * medialRollForce, 0, 0);
            }

            if (lateralThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = lateralThrustAction.ReadValue<float>();
                _rigidBody.AddForce(transform.right * thrust * lateralThrustForce, ForceMode.Impulse);
            }

            if (verticalThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = verticalThrustAction.ReadValue<float>();
                _rigidBody.AddForce(transform.up * thrust * verticalThrustForce, ForceMode.Impulse);

            }
        }

    }
}
