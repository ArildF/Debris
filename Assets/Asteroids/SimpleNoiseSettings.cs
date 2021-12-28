using System;
using UnityEngine;

namespace Asteroids
{
    [Serializable]
    public class SimpleNoiseSettings
    {
        public float strength = 1;
        [Range(1, 8)]
        public int numberOfLayers = 1;

        public float baseRoughness = 1;
        public float roughness = 2;

        public float persistence = 0.5f;

        public Vector3 centre;
        public float minValue;
    }
}