using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroids;
    public float spawnSphereDiameter = 40_000;
    public int initialSpawnNumber = 300;

    public Vector3 move = new Vector3(0, 0, 1);

    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < initialSpawnNumber; i++)
        {
            var index = Random.Range(0, asteroids.Length - 1);
            var asteroid = asteroids[index];
            var position = spawnSphereDiameter * Random.insideUnitSphere;
            var rotation = Random.rotation;
            var spawnedAsteroid = Instantiate(asteroid, position, rotation, transform);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= move * Time.deltaTime;
    }
}
