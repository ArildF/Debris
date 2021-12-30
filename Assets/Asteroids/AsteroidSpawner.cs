using System.Threading.Tasks;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        public GameObject[] asteroids;
        public float spawnSphereDiameter = 40_000;
        public int initialSpawnNumber = 300;
        public float minRotationForce = 0.1f;
        public float maxRotationForce = 1.5f;
        public float maxForce = 40000;
        public float minForce = 5000;
        public float scaleMax = 10;
        public float scaleMin = 0.5f;
        


        // Start is called before the first frame update
        async void Start()
        {
            for (int i = 0; i < initialSpawnNumber; i++)
            {
                var index = Random.Range(0, asteroids.Length - 1);
                var asteroid = asteroids[index];
                var position = spawnSphereDiameter * Random.insideUnitSphere;
                var rotation = Random.rotation;
                var spawnedAsteroid = Instantiate(asteroid, Vector3.zero, Quaternion.identity, transform);

                var asteroidMesh = spawnedAsteroid.GetComponent<AsteroidMesh>();
                await asteroidMesh.CreateMeshAsync();

                spawnedAsteroid.transform.position = position;
                spawnedAsteroid.transform.rotation = rotation;
                
                float radius = asteroidMesh.shapeSettings.radius;
                float volume = (4f / 3) * Mathf.PI * radius;
                float mass = volume * 3.2f * 1000;
                
                
                // float RandomScale() => Random.Range(scaleMin, scaleMax);
                // spawnedAsteroid.transform.localScale = new Vector3(RandomScale(), RandomScale(), RandomScale());

                var rigidBody = spawnedAsteroid.AddComponent<Rigidbody>();
                rigidBody.useGravity = false;
                rigidBody.angularDrag = 0;
                rigidBody.drag = 0;
                rigidBody.mass = mass;
                rigidBody.AddTorque(Random.onUnitSphere * Random.Range(minRotationForce, maxRotationForce), 
                    ForceMode.Impulse);
                rigidBody.AddForce(Random.onUnitSphere * Random.Range(minForce, maxForce));
                
            }
        }
    }
}
