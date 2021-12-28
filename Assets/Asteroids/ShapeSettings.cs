using System;
using UnityEngine;

namespace Asteroids
{
    [Serializable]
    public class ShapeSettings 
    {
        public float radius = 1f;
        public NoiseLayerSettings[] noiseLayers;
    }
}