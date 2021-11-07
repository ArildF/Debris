using System.Collections;
using HUD;
using UniDi;
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
        public InputAction brakeAction;
    
        public float forwardForce = 1_000_000;
        public float reverseForce = 500_000;
        public float lateralRollForce = 100_000;
        public float medialRollForce = 100_000;
        public float lateralThrustForce = 100_000;
        public float verticalThrustForce = 100_000;
        public float brakeRotationSpeed = 0.02f;
        private Rigidbody _rigidBody;
        private InputAction[] _actions;
        private HudInfo _hudInfo;
        private bool _currentlyBraking;
        private ThrustInfo _thrustInfo;

        [Inject]
        public void Init(HudInfo hudInfo, ThrustInfo thrustInfo)
        {
            _hudInfo = hudInfo;
            _thrustInfo = thrustInfo;
        }

        // Start is called before the first frame update
        void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _hudInfo.AbsoluteVelocity = _rigidBody.velocity.magnitude;
        }

        private void OnEnable()
        {
            _actions = new[]
            {
                thrustAction, reverseThrustAction, lateralRollAction, medialRollAction, lateralThrustAction,
                verticalThrustAction, brakeAction,
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
            _thrustInfo.CurrentDirectionalThrust = _thrustInfo.CurrentRotationalThrust = 0;
            
            if (thrustAction.phase == InputActionPhase.Started)
            {
                _thrustInfo.CurrentDirectionalThrust = forwardForce;
                _rigidBody.AddForce(transform.forward * forwardForce, ForceMode.Impulse); 
            }
            if (reverseThrustAction.phase == InputActionPhase.Started)
            {
                _thrustInfo.CurrentDirectionalThrust = reverseForce;
                _rigidBody.AddForce(transform.forward * -reverseForce, ForceMode.Impulse); 
            }

            if (lateralRollAction.phase == InputActionPhase.Started)
            {
                var roll = lateralRollAction.ReadValue<float>();
                _thrustInfo.CurrentRotationalThrust = roll * lateralRollForce;
                _rigidBody.AddRelativeTorque(0, 0, -roll * lateralRollForce);
            }
        
            if (medialRollAction.phase == InputActionPhase.Started)
            {
                var roll = medialRollAction.ReadValue<float>();
                _thrustInfo.CurrentRotationalThrust = roll * medialRollForce;
                _rigidBody.AddRelativeTorque(roll * medialRollForce, 0, 0);
            }

            if (lateralThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = lateralThrustAction.ReadValue<float>();
                _thrustInfo.CurrentDirectionalThrust = thrust * lateralThrustForce;
                _rigidBody.AddForce(transform.right * (thrust * lateralThrustForce), ForceMode.Impulse);
            }

            if (verticalThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = verticalThrustAction.ReadValue<float>();
                _thrustInfo.CurrentDirectionalThrust = thrust * verticalThrustForce;
                _rigidBody.AddForce(transform.up * (thrust * verticalThrustForce), ForceMode.Impulse);
            }

            if (!_currentlyBraking && brakeAction.phase == InputActionPhase.Started)
            {
                StartCoroutine(Brake());
            }
        }

        private IEnumerator Brake()
        {
            _currentlyBraking = true;
            try
            {
                var shipTransform = transform;
                var velocityRotation = Quaternion.LookRotation(-_rigidBody.velocity, shipTransform.up);
                while (brakeAction.phase == InputActionPhase.Started && 
                       Quaternion.Angle(shipTransform.rotation, velocityRotation) > float.Epsilon)
                {
                    transform.rotation = Quaternion.Slerp(shipTransform.rotation, velocityRotation, Time.fixedTime * brakeRotationSpeed);
                    yield return new WaitForFixedUpdate();
                }
            
                Debug.Log("Finished rotation phase of braking");
                while (brakeAction.phase == InputActionPhase.Started &&
                       _rigidBody.velocity.magnitude > 50)
                {
                    _thrustInfo.CurrentDirectionalThrust = forwardForce;
                    _rigidBody.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
                    yield return new WaitForFixedUpdate();
                }
                
                Debug.Log("Finished force phase of braking");
            }
            finally
            {
                _currentlyBraking = false;
            }
            
        }
    }
}
