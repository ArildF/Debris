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
                print($"Unprocessed reticle screen point {point}");

                // if (point.z < 0)
                // {
                //     point *= -1;
                // }
                point.x = Mathf.Clamp(point.x, 0, camera.pixelWidth);
                point.y = Mathf.Clamp(point.y, 0, camera.pixelHeight);

                var calculated = CalculateFrustumIntersection();

                if (point != original || point.z < 0)
                {
                    point = calculated;
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

                // var start = _playerViewInfo.ShipTransform.position + _playerViewInfo.ShipTransform.forward * 50;
                // // Debug.DrawLine(start, _targetInfo.Target.position, Color.green);
                // camera.CalculateFrustumCorners();
                print($"Processed reticle screen point {point}");
                _reticleCircle.transform.position = (Vector2)point;
            }

        }

        private Vector3 CalculateFrustumIntersection()
        {
            var player = _playerViewInfo.ShipTransform;
            var colors = new[] { Color.blue, Color.green, Color.magenta, Color.red };
            var start = player.position + player.forward * 50;

            // for (int i = 0; i < frustumCorners.Length; i++)
            // {
            //     // Debug.DrawLine(start, frustumCorners[i], colors[i]);
            // }
            
            Debug.DrawLine(start, _targetInfo.Target.position, Color.red);

            var direction = (_targetInfo.Target.position - start);
            var projectedDirection = Quaternion.Inverse(player.rotation) * Vector3.ProjectOnPlane(direction, _playerViewInfo.ShipTransform.forward);

            print($"Projected direction: {projectedDirection}");
            Debug.DrawRay(start, projectedDirection * 1000, Color.magenta);

            var planes = new[]
            {
                new Plane(Vector3.right, new Vector3(0, 0, 0)),
                new Plane(Vector3.down, new Vector3(0, camera.pixelHeight, 0)),
                new Plane(Vector3.left, new Vector3(camera.pixelWidth, camera.pixelHeight, 0)),
                new Plane(Vector3.up, new Vector3(camera.pixelWidth, 0, 0)),
            };

            var centerScreen = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);
            projectedDirection.z = 0;
            var ray = new Ray(centerScreen, projectedDirection);
            print($"Ray: {ray}");

            Vector3 closest = Vector3.zero;
            for (int i = 0; i < planes.Length; i++)
            {
                var plane = planes[i];
                
                // DebugDrawPlane(plane, colors[i]);
                
                if (plane.Raycast(ray, out float enter))
                {
                    var intersect = ray.GetPoint(enter);
                    
                    print($"Intersected with frustum plane {i} at ss {intersect}");
                    // Debug.DrawLine(start, intersect, Color.yellow);

                    if (closest == Vector3.zero || (intersect - centerScreen).magnitude < closest.magnitude)
                    {
                        closest = intersect;
                    }

                }
            }

            if (closest != Vector3.zero)
            {
                print($"Closest: {closest}");
                return closest;
            }
            throw new Exception("Really should not happen.");

        }

        private void DebugDrawPlane(Plane plane, Color color)
        {
            const int planeSize = 100;
            var player = _playerViewInfo.ShipTransform;
            const int halfPlane = planeSize / 2;
            var origin = player.position + player.forward * halfPlane - plane.normal.normalized * plane.distance;

            var forward = player.forward;
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