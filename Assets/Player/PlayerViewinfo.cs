using UnityEngine;

namespace Player
{
    public class PlayerViewInfo
    {
        public ViewDirection ViewDirection { get; set; }
        public Vector3 UpDirection { get; set; }
        public Transform ShipTransform { get; set; }

        public Quaternion ViewRotation => ViewDirection switch
        {
            ViewDirection.Front => Quaternion.identity,
            ViewDirection.Back => Quaternion.AngleAxis(180, Vector3.up),
            ViewDirection.Left => Quaternion.AngleAxis(90, Vector3.up),
            ViewDirection.Right => Quaternion.AngleAxis(-90, Vector3.up),
            ViewDirection.Top => Quaternion.AngleAxis(90, Vector3.right),
            ViewDirection.Bottom => Quaternion.AngleAxis(-90, Vector3.right),
            _ => Quaternion.identity,
        };
    }

    public enum ViewDirection
    {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }
}