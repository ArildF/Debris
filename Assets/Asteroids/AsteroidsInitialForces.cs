using System.Linq;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidsInitialForces : MonoBehaviour
    {
        public float minRotationForce = 0.1f;
        public float maxRotationForce = 1.5f;
        public float maxForce = 40000;
        public float minForce = 5000;
    
        // Start is called before the first frame update
        void Start()
        {

            foreach (Transform child in transform)
            {
                var asteroidMesh = child.GetComponent<AsteroidMesh>();
                float radius = asteroidMesh.shapeSettings.radius;
                float volume = (4f / 3) * Mathf.PI * radius;
                float mass = volume * 3.2f * 1000;


                // float RandomScale() => Random.Range(scaleMin, scaleMax);
                // spawnedAsteroid.transform.localScale = new Vector3(RandomScale(), RandomScale(), RandomScale());

                float rotationMassCoefficient = mass / 10000000;

                var rigidBody = child.gameObject.AddComponent<Rigidbody>();
                rigidBody.useGravity = false;
                rigidBody.angularDrag = 0;
                rigidBody.drag = 0;
                rigidBody.mass = mass;
                rigidBody.AddTorque(Random.onUnitSphere * 
                                    Random.Range(minRotationForce * rotationMassCoefficient, 
                                        maxRotationForce * rotationMassCoefficient),
                    ForceMode.Impulse);
                rigidBody.AddForce(Random.onUnitSphere * Random.Range(minForce * mass, maxForce * mass));

                var collider = child.gameObject.AddComponent<MeshCollider>();
                collider.convex = true;
                collider.cookingOptions = MeshColliderCookingOptions.None;

                var lodChildMesh = child.Cast<Transform>().Reverse()
                    .Select(t => t.gameObject.GetComponent<MeshFilter>().sharedMesh)
                    .Where(m => m.triangles.Length > 3)
                    .Skip(1).FirstOrDefault();
                if (lodChildMesh != null)
                {
                    print(
                        $"collider mesh vertices {lodChildMesh.vertexCount} faces {lodChildMesh.triangles.Length}");
                    collider.sharedMesh = lodChildMesh;
                }
                else
                {
                    print("Unable to find LOD child");
                }
            }
        
        }
    }
}
