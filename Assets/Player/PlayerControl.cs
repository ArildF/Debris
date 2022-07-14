using System.Collections;
using System.Linq;
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
        public InputAction targetAction;
    
        public float forwardForce = 1_000_000;
        public float reverseForce = 500_000;
        public float lateralRollForce = 100_000;
        public float medialRollForce = 100_000;
        public float lateralThrustForce = 100_000;
        public float verticalThrustForce = 100_000;
        public float brakeRotationSpeed = 0.02f;

        public AnimationCurve _thrustCurve;

        public float targetingSphereDiameter = 400;
        
        public Transform shipCamera;
        public Rigidbody rigidBody;
        private InputAction[] _actions;
        private HudInfo _hudInfo;
        private bool _currentlyBraking;
        public ThrustInfo thrustInfo;
        private Vector2 _dpadDirection;
        private PlayerViewInfo _playerViewInfo;
        private TargetInfo _targetInfo;

        [Inject]
        public void Init(HudInfo hudInfo, ThrustInfo thrustInfo, PlayerViewInfo playerViewInfo, TargetInfo targetInfo)
        {
            _hudInfo = hudInfo;
            this.thrustInfo = thrustInfo;
            _playerViewInfo = playerViewInfo;
            _targetInfo = targetInfo;
        }

        // Start is called before the first frame update
        private void Start()
        {
            _playerViewInfo.ShipTransform = transform;
            rigidBody = GetComponent<Rigidbody>();

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

            targetAction.performed += _ => TryTarget();
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
            _hudInfo.AbsoluteVelocity = rigidBody.velocity.magnitude;
        }

        private void OnEnable()
        {
            _actions = new[]
            {
                thrustAction, reverseThrustAction, lateralRollAction, medialRollAction, lateralThrustAction,
                verticalThrustAction, brakeAction, cameraControl, targetAction,
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
            thrustInfo.CurrentDirectionalThrust = thrustInfo.CurrentRotationalThrust = 0;
            
            if (thrustAction.phase == InputActionPhase.Started)
            {
                var input = thrustAction.ReadValue<float>();
                var force = Abs(input) * _thrustCurve.Evaluate(input);
                _playerViewInfo.CurrentView.PrimaryThrust(this, force);
            }
            if (reverseThrustAction.phase == InputActionPhase.Started)
            {
                var input = reverseThrustAction.ReadValue<float>();
                var force = Abs(input) * _thrustCurve.Evaluate(input);
                _playerViewInfo.CurrentView.PrimaryReverseThrust(this, force);
            }

            if (lateralRollAction.phase == InputActionPhase.Started)
            {
                var roll = lateralRollAction.ReadValue<float>();
                _playerViewInfo.CurrentView.LateralRoll(this, roll);
            }
        
            if (medialRollAction.phase == InputActionPhase.Started)
            {
                var roll = medialRollAction.ReadValue<float>();
                _playerViewInfo.CurrentView.MedialRoll(this, roll);
            }

            if (lateralThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = lateralThrustAction.ReadValue<float>();
                _playerViewInfo.CurrentView.LateralThrust(this, thrust);
            }

            if (verticalThrustAction.phase == InputActionPhase.Started)
            {
                var thrust = verticalThrustAction.ReadValue<float>();
                _playerViewInfo.CurrentView.VerticalThrust(this, thrust);
            }
            

            if (!_currentlyBraking && brakeAction.phase == InputActionPhase.Started)
            {
                StartCoroutine(Brake());
            }
        }

        private void TryTarget()
        {
            print($"Trying to target");
            var hits = Physics.SphereCastAll(transform.position, targetingSphereDiameter, transform.forward);
            print($"Got {hits.Length} hits from sphere cast");
            var target = hits.Select(h => h.transform).FirstOrDefault(t => t.transform != _targetInfo.Target);
            if (target != null)
            {
                print($"Acquired target{target.transform.name}");
                _targetInfo.Target = target.transform;
            }
            else
            {
                print("Found no targets");
                _targetInfo.Target = null;
            }
        }

        private IEnumerator Brake()
        {
            _currentlyBraking = true;
            try
            {
                var shipTransform = transform;
                var velocityRotation = Quaternion.LookRotation(-rigidBody.velocity, shipTransform.up);
                while (brakeAction.phase == InputActionPhase.Started && 
                       Quaternion.Angle(shipTransform.rotation, velocityRotation) > float.Epsilon)
                {
                    transform.rotation = Quaternion.Slerp(shipTransform.rotation, velocityRotation, Time.fixedTime * brakeRotationSpeed);
                    yield return new WaitForFixedUpdate();
                }
            
                Debug.Log("Finished rotation phase of braking");
                while (brakeAction.phase == InputActionPhase.Started &&
                       rigidBody.velocity.magnitude > 50)
                {
                    thrustInfo.CurrentDirectionalThrust = forwardForce;
                    rigidBody.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
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
