using System;
using System.Linq;
using UnityEngine;

namespace Asteroids
{
    public class ShapeGenerator
    {
        private readonly ShapeSettings _shapeSettings;
        private readonly INoiseFilter[] _noiseFilters;

        public ShapeGenerator(ShapeSettings shapeSettings)
        {
            _shapeSettings = shapeSettings;

            _noiseFilters = _shapeSettings.noiseLayers.Select(nl => nl.filterType switch
            {
                NoiseLayerSettings.FilterType.Simple => (INoiseFilter)new SimpleNoiseFilter(nl.simpleNoiseSettings),
                NoiseLayerSettings.FilterType.Ridged => new RidgedNoiseFilter(nl.ridgedNoiseSettings),
                _ => throw new Exception("Eh?")
            }).ToArray();
        }

        public Vector3 CalculatePoint(Vector3 pointOnUnitSphere)
        {
            float elevation = 0f;
            for (int i = 0; i < _noiseFilters.Length; i++)
            {
                var filter = _noiseFilters[i];
                if (_shapeSettings.noiseLayers[i].enabled)
                {
                    elevation += filter.Evaluate(pointOnUnitSphere);
                }
            }

            return pointOnUnitSphere * _shapeSettings.radius * (1 + elevation);
        }
    }
}