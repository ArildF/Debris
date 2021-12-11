using System.Collections;
using HUD;
using UniDi;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Mathf;

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
        public InputAction cameraControl;
    
        public float forwardForce = 1_000_000;
        public float reverseForce = 500_000;
        public float lateralRollForce = 100_000;
        public float medialRollForce = 100_000;
        public float lateralThrustForce = 100_000;
        public float verticalThrustForce = 100_000;
        public float brakeRotationSpeed = 0.02f;
        public Transform shipCamera;
        private Rigidbody _rigidBody;
        private InputAction[] _actions;
        private HudInfo _hudInfo;
        private bool _currentlyBraking;
        private ThrustInfo _thrustInfo;
        private Vector2 _dpadDirection;
        private PlayerViewInfo _playerViewInfo;

        [Inject]
        public void Init(HudInfo hudInfo, ThrustInfo thrustInfo, PlayerViewInfo playerViewInfo)
        {
            _hudInfo = hudInfo;
            _thrustInfo = thrustInfo;
            _playerViewInfo = playerViewInfo;
        }

        // Start is called before the first frame update
        private void Start()
        {
            _playerViewInfo.ShipTransform = transform;
            _rigidBody = GetComponent<Rigidbody>();

            cameraControl.started += ctx => _dpadDirection = ctx.ReadValue<Vector2>();

            cameraControl.canceled += ctx =>
            {
                if (_dpadDirection.magnitude > Epsilon)
                {
                    var dir = (_dpadDirection.x, _dpadDirection.y) switch
                    {
                        var (x, _) when x < -Epsilon => ViewDirection.Left,
                        var (x, _) when x > Epsilon => ViewDirection.Right,
                        var (_, y) when y > Epsilon => ViewDirection.Front,
                        var (_, y) when y < Epsilon => ViewDirection.Back,
                        _ => ViewDirection.Front
                    };
                    _playerViewInfo.ViewDirection = dir;
                    Debug.Log($"Setting UpDirection to {_playerViewInfo.UpDirection}");
                    _playerViewInfo.UpDirection = transform.up;
                    
                    _dpadDirection = Vector2.zero;
                }
            };
            cameraControl.started += ctx => Debug.Log("up down started");
            cameraControl.performed += ctx =>
            {
                if (Abs(_dpadDirection.y) > Epsilon)
                {
                    Debug.Log("up down performed");
                    var transformActual = transform;
                    var forward = transformActual.forward;
                    (_playerViewInfo.ViewDirection, _playerViewInfo.UpDirection) = 
                        (_dpadDirection.x, _dpadDirection.y) switch
                    {
                        var (_, y) when y > Epsilon => (ViewDirection.Top, -forward),
                        var (_, y) when y < Epsilon => (ViewDirection.Bottom, forward),
                        _ => (ViewDirection.Front, transformActual.up),
                    };
                    _dpadDirection = Vector2.zero;
                }
            };
        }

        private void Update()
        {
            var playerTransform = transform;
            var forward = playerTransform.forward;
            _playerViewInfo.UpDirection = _playerViewInfo.ViewDirection switch
            {
                ViewDirection.Top => -forward,
                ViewDirection.Bottom => forward,
                _ => playerTransform.up,
            };
            _hudInfo.AbsoluteVelocity = _rigidBody.velocity.magnitude;
        }

        private void OnEnable()
        {
            _actions = new[]
            {
                thrustAction, reverseThrustAction, lateralRollAction, medialRollAction, lateralThrustAction,
                verticalThrustAction, brakeAction, cameraControl, 
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
                var force = Abs(thrustAction.ReadValue<float>()) * forwardForce;
                _thrustInfo.CurrentDirectionalThrust = force;
                _rigidBody.AddForce(transform.forward * force, ForceMode.Impulse); 
            }
            if (reverseThrustAction.phase == InputActionPhase.Started)
            {
                var force = Abs(reverseThrustAction	.ReadValue<float>()) * reverseForce;
                _thrustInfo.CurrentDirectionalThrust = force;
                _rigidBody.AddForce(transform.forward * -force, ForceMode.Impulse); 
            }

            if (lateralRollAction.phase == InputActionPhase.Started)
            {
                var roll = lateralRollAction.ReadValue<float>();
                _thrustInfo.CurrentRotationalThrust = Abs(roll * lateralRollForce);
                _rigidBody.AddRelativeTorque(0, 0, -roll * lateralRollForce);
            }
        
            if (medialRollAction.phase == InputActionPhase.Started)
            {
                var roll = medialRollAction.ReadValue<float>();
                _thrustInfo.CurrentRotationalThrust = Max(_thrustInfo.CurrentRotationalThrust, Abs(roll * medialRollForce));
                _rigidBody.AddRelativeTorque(roll * medialRollForce, 0, 0);
            }

            if (lateralThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = lateralThrustAction.ReadValue<float>();
                _thrustInfo.CurrentDirectionalThrust = Abs(thrust * lateralThrustForce);
                _rigidBody.AddForce(transform.right * (thrust * lateralThrustForce), ForceMode.Impulse);
            }

            if (verticalThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = verticalThrustAction.ReadValue<float>();
                _thrustInfo.CurrentDirectionalThrust = Max(_thrustInfo.CurrentDirectionalThrust, Abs(thrust * verticalThrustForce));
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
