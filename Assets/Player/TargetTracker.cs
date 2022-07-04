using System;
using Player.UI;
using UniDi;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(ReticleCircle))]
    public class TargetTracker : MonoBehaviour
    {
        private TargetInfo _targetInfo;
        private ReticleCircle _reticleCircle;

        public Camera camera;

        [Inject]
        public void Init(TargetInfo targetInfo)
        {
            _targetInfo = targetInfo;
        }

        private void Start()
        {
            _reticleCircle = GetComponent<ReticleCircle>();
        }

        private void Update()
        {
            _reticleCircle.enabled = _targetInfo.Target != null;
            if (_reticleCircle.enabled)
            {
                Vector2 worldToScreenPoint = camera.WorldToScreenPoint(_targetInfo.Target.position);
                _reticleCircle.transform.position = worldToScreenPoint;
            }

        }
    }
}