using System;
using JetBrains.Annotations;
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

        public new Camera camera;
        public Transform manualTarget;
        private PlayerViewInfo _playerViewInfo;

        [Inject]
        public void Init(TargetInfo targetInfo, PlayerViewInfo playerViewInfo)
        {
            _targetInfo = targetInfo;
            _playerViewInfo = playerViewInfo;
        }

        private void Start()
        {
            _reticleCircle = GetComponent<ReticleCircle>();
            _targetInfo.Target = manualTarget;
        }

        private void Update()
        {
            _reticleCircle.enabled = _targetInfo.Target != null;
            if (_reticleCircle.enabled)
            {
                Vector3 point = camera.WorldToScreenPoint(_targetInfo.Target.position);
                Vector3 original = point;

                point.x = Mathf.Clamp(point.x, 0, camera.pixelWidth);
                point.y = Mathf.Clamp(point.y, 0, camera.pixelHeight);

                if (point != original || point.z < 0)
                {
                    point = CalculateScreenEdgeIntersection();
                    if (!_reticleCircle.fill)
                    {
                        _reticleCircle.fill = true;
                        _reticleCircle.SetVerticesDirty();
                    }
                }
                else if((point == original && point.z >= 0) && _reticleCircle.fill)
                {
                    _reticleCircle.fill = false;
                    _reticleCircle.SetVerticesDirty();
                }

                _reticleCircle.transform.position = (Vector2)point;
            }

        }

        private Vector3 CalculateScreenEdgeIntersection()
        {
            var player = _playerViewInfo.ShipTransform;
            var start = player.position + player.forward * 50;

            var direction = _targetInfo.Target.position - start;
            var projectedDirection = Quaternion.Inverse(player.rotation) * Vector3.ProjectOnPlane(direction, _playerViewInfo.ShipTransform.forward);

            var height = camera.pixelHeight;
            var width = camera.pixelWidth;
            var planes = new[]
            {
                new Plane(Vector3.right, new Vector3(0, 0, 0)),
                new Plane(Vector3.down, new Vector3(0, height, 0)),
                new Plane(Vector3.left, new Vector3(width, height, 0)),
                new Plane(Vector3.up, new Vector3(width, 0, 0)),
            };

            var centerScreen = new Vector3(width / 2f, height / 2f, 0);
            projectedDirection.z = 0;
            var ray = new Ray(centerScreen, projectedDirection);

            Vector3 closest = Vector3.zero;
            for (var i = 0; i < planes.Length; i++)
            {
                var plane = planes[i];
                
                if (plane.Raycast(ray, out float enter))
                {
                    var intersect = ray.GetPoint(enter);
                    
                    if (closest == Vector3.zero || (intersect - centerScreen).magnitude < closest.magnitude)
                    {
                        closest = intersect;
                    }
                }
            }

            if (closest != Vector3.zero)
            {
                return closest;
            }
            throw new Exception("Really should not happen.");

        }

        [UsedImplicitly]
        private void DebugDrawPlane(Plane plane, Color color)
        {
            const int planeSize = 100;
            var player = _playerViewInfo.ShipTransform;
            const int halfPlane = planeSize / 2;
            var forward = player.forward;
            var origin = player.position + forward * halfPlane - plane.normal.normalized * plane.distance;

            var perp = Vector3.Cross(forward, plane.normal);
            Debug.DrawLine(origin - perp * halfPlane - forward * halfPlane, 
                origin - perp * halfPlane + forward * halfPlane, color);
            Debug.DrawLine(origin - perp * halfPlane + forward * halfPlane, 
                origin + perp * halfPlane + forward * halfPlane, color);
            Debug.DrawLine(origin + perp * halfPlane + forward * halfPlane, 
                origin + perp * halfPlane - forward * halfPlane, color);
            Debug.DrawLine(origin + perp * halfPlane - forward * halfPlane, 
                origin - perp * halfPlane - forward * halfPlane, color);
        }
    }
}