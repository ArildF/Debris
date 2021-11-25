using UniDi;
using UnityEngine;

namespace Player
{
    public class CameraControl : MonoBehaviour
    {
        private PlayerViewInfo _playerViewInfo;

        [Inject]
        public void Init(PlayerViewInfo playerViewInfo)
        {
            _playerViewInfo = playerViewInfo;
        }

        void Update()
        {
            var t = _playerViewInfo.ShipTransform;
            var forward = t.forward;
            var right = t.right;
            var up = t.up;
            var dir = _playerViewInfo.ViewDirection switch
            {
                ViewDirection.Front => forward,
                ViewDirection.Back => -forward,
                ViewDirection.Left => -right,
                ViewDirection.Right => right,
                ViewDirection.Top => up,
                ViewDirection.Bottom => -up,
                _ => Vector3.forward
            };
            transform.rotation = Quaternion.LookRotation(dir, up);
        }
    }
}