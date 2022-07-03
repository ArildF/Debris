using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Base;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        public SpawnPrefabEntry[] asteroids;
        public float spawnSphereDiameter = 40_000;
        public int initialSpawnNumber = 300;

        public float scaleMax = 10;
        public float scaleMin = 0.5f;
        public int batchSize = 32;
        public int maxInFlight = 7;

        [Range(0, 100f)]
        public float progress = 0f;
        

        [ContextMenu("Spawn asteroids")]
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

            var guids = AssetDatabase.FindAssets("", new[] { "Assets/__DynAsteroids" });
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath);

            var failedPaths = new List<string>();
            if (!AssetDatabase.DeleteAssets(paths.ToArray(), failedPaths))
            {
                print($"Failed to delete {string.Join(", ", failedPaths)}");
            }
        }
        

        private async Task SpawnAsteroids()
        {
            var sw = Stopwatch.StartNew();

            progress = 0f;

            var indexMap = new List<int>();
            foreach (var (asteroid, index) in asteroids.Select((a, idx)=> (a, idx)))
            {
                for (int i = 0; i < asteroid.weight; i++)
                {
                    indexMap.Add(index);
                }
            }

            async Task DoSpawn()
            {
                
                var indexOfIndex = Random.Range(0, indexMap.Count);
                var index = indexMap[indexOfIndex];
                var asteroid = asteroids[index].asteroid;
                
                print($"indexOfIndex: {indexOfIndex}, index: {index}, asteroid: {asteroid.name}");
                
                var position = spawnSphereDiameter * Random.insideUnitSphere;
                var rotation = Random.rotation;
                var spawnedAsteroid = Instantiate(asteroid, Vector3.zero, Quaternion.identity, transform);

                var asteroidMesh = spawnedAsteroid.GetComponent<AsteroidMesh>();
                await asteroidMesh.CreateMeshAsync();

                spawnedAsteroid.transform.position = position;
                spawnedAsteroid.transform.rotation = rotation;

                // float RandomScale() => Random.Range(scaleMin, scaleMax);
                // spawnedAsteroid.transform.localScale = new Vector3(RandomScale(), RandomScale(), RandomScale());
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
