using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        public int batchSize = 32;
        public int maxInFlight = 7;

        [Range(0, 100f)]
        public float progress = 0f;
        


        // Start is called before the first frame update
        async void Start()
        {
            // await SpawnAsteroids();
        }

        [ContextMenu("Spawn manually")]
        public async void SpawnManually()
        {
            await SpawnAsteroids();
        }

        [ContextMenu("Delete asteroids")]
        public void DeleteAsteroids()
        {
            var children = transform.Cast<Transform>().ToArray();
            foreach (var child in children)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private async Task SpawnAsteroids()
        {
            var sw = Stopwatch.StartNew();

            progress = 0f;

            async Task DoSpawn()
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

                progress += 100f / initialSpawnNumber;
            }

            var semaphore = new SemaphoreSlim(maxInFlight, maxInFlight);

            async Task DoSpawnLimited()
            {
                await semaphore.WaitAsync();
                try
                {
                    await DoSpawn();
                }
                finally
                {
                    semaphore.Release();
                }
            }

            var taskBatches = Enumerable.Range(0, initialSpawnNumber).BatchBy(batchSize);
            foreach (var batch in taskBatches.Select(b => b.Select(_ => DoSpawnLimited())))
            {
                await Task.WhenAll(batch);
            }

            // for (int i = 0; i < initialSpawnNumber; i++)
            // {
            //     await DoSpawn(i);
            // }
            
            Debug.Log($"Instantiated {initialSpawnNumber} asteroids in {sw.Elapsed}");
        }
    }
}
