using Base;
using UnityEngine;

namespace Asteroids
{
    public class RidgedNoiseFilter : INoiseFilter
    {
        private readonly RidgedNoiseSettings _ridgedNoiseSettings;
        private readonly Noise _noise = new Noise();

        public RidgedNoiseFilter(RidgedNoiseSettings ridgedNoiseSettings)
        {
            _ridgedNoiseSettings = ridgedNoiseSettings;
        }

        public float Evaluate(Vector3 pointOnUnitSphere)
        {
            float noiseValue = 0;
            float frequency = _ridgedNoiseSettings.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            for (int i = 0; i < _ridgedNoiseSettings.numberOfLayers; i++)
            {
                float v = 1 - Mathf.Abs(_noise.Evaluate(pointOnUnitSphere * frequency + _ridgedNoiseSettings.centre));

                v *= v;
                v *= weight;

                weight = Mathf.Clamp01(v * _ridgedNoiseSettings.weightMultiplier);

                noiseValue += v * amplitude;
                frequency += _ridgedNoiseSettings.roughness;
                amplitude += _ridgedNoiseSettings.persistence;
            }

            noiseValue = Mathf.Max(0, noiseValue - _ridgedNoiseSettings.minValue);

            return noiseValue * _ridgedNoiseSettings.strength;
        }
    }
}