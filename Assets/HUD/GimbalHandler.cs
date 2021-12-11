using Player;
using UI.ThreeDimensional;
using UniDi;
using UnityEngine;

namespace HUD
{
    public class GimbalHandler : MonoBehaviour
    {
        public Rigidbody shipRigidBody;

        private UIObject3D _uiObject;
        private Vector3 _lastRigidBodyVelocity;
        private Quaternion _lastRotation;
        private PlayerViewInfo _playerViewInfo;
        private ViewDirection _lastViewDirection;

        [Inject]
        public void Init(PlayerViewInfo playerViewInfo)
        {
            _playerViewInfo = playerViewInfo;
        }
        // Start is called before the first frame update
        void Start()
        {
            _uiObject = GetComponent<UIObject3D>();
        }

        // Update is called once per frame
        void Update()
        {
            var rigidBodyVelocity = shipRigidBody.velocity;
            var rotation = shipRigidBody.transform.rotation;
            var viewDirection = _playerViewInfo.ViewDirection;
            if (Vector3.Distance(_lastRigidBodyVelocity, rigidBodyVelocity) > float.Epsilon ||
                Quaternion.Angle(_lastRotation, rotation) > float.Epsilon ||
                viewDirection != _lastViewDirection)
            {
                var rigidBodyTransform = shipRigidBody.transform;
                var rigidbodyDirection = rigidBodyVelocity.sqrMagnitude > float.Epsilon ? rigidBodyVelocity.normalized : rigidBodyTransform.forward;
                var shipVelocityRotation = Quaternion.LookRotation(rigidbodyDirection, rigidBodyTransform.up);

                var velocityRotation =
                    Quaternion.Inverse(
                        rotation
                        ) * shipVelocityRotation;

                var viewRotation = _playerViewInfo.ViewDirection switch
                {
                    ViewDirection.Front => Quaternion.identity,
                    ViewDirection.Back => Quaternion.AngleAxis(180, Vector3.up),
                    ViewDirection.Left => Quaternion.AngleAxis(90, Vector3.up),
                    ViewDirection.Right => Quaternion.AngleAxis(-90, Vector3.up),
                    ViewDirection.Top => Quaternion.AngleAxis(90, Vector3.right),
                    ViewDirection.Bottom => Quaternion.AngleAxis(-90, Vector3.right),
                    _ => Quaternion.identity,
                };

                var finalRotation = viewRotation * velocityRotation ;
                
                _uiObject.targetContainer.rotation = finalRotation;

                _uiObject.UpdateDisplay();
                
                _lastRigidBodyVelocity = rigidBodyVelocity;
                _lastRotation = rotation;
                _lastViewDirection = _playerViewInfo.ViewDirection;
            }
        }
    }
}