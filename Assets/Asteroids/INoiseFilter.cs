using UnityEngine;

namespace Asteroids
{
    public interface INoiseFilter
    {
        float Evaluate(Vector3 pointOnUnitSphere);
    }
}