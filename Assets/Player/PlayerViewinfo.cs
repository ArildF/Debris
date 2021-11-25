using UnityEngine;

namespace Player
{
    public class PlayerViewInfo
    {
        public ViewDirection ViewDirection { get; set; }
        public Vector3 UpDirection { get; set; }
        public Transform ShipTransform { get; set; }
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