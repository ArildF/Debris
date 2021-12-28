using System;

namespace Asteroids
{
    [Serializable]
    public class RidgedNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultiplier = .8f;
    }
}