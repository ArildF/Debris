using UI.ThreeDimensional;
using UnityEngine;

namespace HUD
{
    public class GimbalHandler : MonoBehaviour
    {
        public Rigidbody shipRigidBody;

        private UIObject3D _uiObject;
        private Vector3 _lastRigidBodyVelocity;
        private Quaternion _lastRotation;

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
            if (Vector3.Distance(_lastRigidBodyVelocity, rigidBodyVelocity) > float.Epsilon ||
                Quaternion.Angle(_lastRotation, rotation) > float.Epsilon)
            {
                var rigidbodyDirection = rigidBodyVelocity.normalized;
                // Debug.Log($"Rigid body is moving {rigidbodyDirection}");
                // Debug.Log($"Ship orientation {rotation.eulerAngles}");
                var shipVelocityRotation = Quaternion.LookRotation(rigidbodyDirection, shipRigidBody.transform.up);
                // Debug.Log($"Ship velocity rotation: {shipVelocityRotation.eulerAngles}");

                var targetContainerRotation =
                    Quaternion.Inverse(
                        rotation
                        ) * shipVelocityRotation;
                // Debug.Log($"Rotation: {targetContainerRotation.eulerAngles}");
                _uiObject.targetContainer.rotation = targetContainerRotation;

                _uiObject.UpdateDisplay();
                _lastRigidBodyVelocity = rigidBodyVelocity;
                _lastRotation = rotation;
            }
            // _uiObject.TargetRotation = rigidBody.velocity.normalized;
            // _uiObject.
        }
    }
}