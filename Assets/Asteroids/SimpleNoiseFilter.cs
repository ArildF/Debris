using Base;
using UnityEngine;

namespace Asteroids
{
    public class SimpleNoiseFilter : INoiseFilter
    { 
        private readonly SimpleNoiseSettings _noiseSettings;
        private readonly Noise _noise;

        public SimpleNoiseFilter(SimpleNoiseSettings noiseSettings)
        {
            _noiseSettings = noiseSettings;
            _noise = new Noise();
        }

        public float Evaluate(Vector3 vector)
        {
            float noiseValue = 0;
            float frequency = _noiseSettings.baseRoughness;
            float amplitude = 1;

            for (int i = 0; i < _noiseSettings.numberOfLayers; i++)
            {
                float v = _noise.Evaluate(vector * frequency + _noiseSettings.centre);
                noiseValue += (v + 1) * 0.5f * amplitude;
                frequency *= _noiseSettings.roughness;
                amplitude *= _noiseSettings.persistence;
            }

            noiseValue = Mathf.Max(noiseValue, _noiseSettings.minValue);

            return noiseValue * _noiseSettings.strength;
        }
    }
}