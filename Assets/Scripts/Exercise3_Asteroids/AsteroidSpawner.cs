using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // Stores Large, Medium, and Small Asteroid Prefabs
    [SerializeField] private GameObject largeAsteroidPrefab;
    [SerializeField] private GameObject mediumAsteroidPrefab;
    [SerializeField] private GameObject smallAsteroidPrefab;

    // Controls how many Large asteroids spawn whe n the game begins.
    [SerializeField] private int initialAsteroidCount = 5;

    // Stores min/max horizontal spawn position.
    [SerializeField] private float spawnXMax = 0f;
    [SerializeField] private float spawnXMin = 0f;

    // Stores min/max vertical spawn postions
    [SerializeField] private float spawnYMax = 0f;
    [SerializeField] private float spawnYMin = 0f;

    // Controls how far the initial asteroids spawn from the center.
    [SerializeField] private float playerSafeDistance = 3;

    void Start()
    {
        // Gets half the cameras vertical viewing area.
        float screenHalfHeight = Camera.main.orthographicSize;
        
        // Calculates half the cameras horizontal viewing area.
        float screenHalfWidth = Camera.main.aspect * screenHalfHeight;

        // Sets the horizontal and vertical spawn boundaries to the camera boundries.
        spawnXMax = screenHalfWidth;
        spawnXMin = -screenHalfWidth;
        spawnYMax = screenHalfHeight;
        spawnYMin = -screenHalfHeight;

        // Creates the starting asteroids.
        SpawnInitialAsteroids();
    }

    void Update()
    {
        
    }

    private void SpawnInitialAsteroids()
    {
        // Repeats once for every asteroid that should spawn.
        for(int i = 0; i < initialAsteroidCount; i++)
        {
            Vector3 spawnPosition;

            // Continues selecting random position until the position is far enough away from the center of the screen.
            do
            {
                // Selects a random horizontal (X) and Vertical (Y) position.
                float randomX = Random.Range(spawnXMin, spawnXMax);
                float randomY = Random.Range(spawnYMin, spawnYMax);
                
                // Calculates the new spawn position
                spawnPosition = new Vector3(randomX, randomY, 0f);
            }
            while(Vector3.Distance(spawnPosition, Vector3.zero) < playerSafeDistance);
            SpawnAsteroid(spawnPosition, Asteroid.AsteroidSize.Large);
        }
    }

    public void SpawnAsteroid(Vector3 position, Asteroid.AsteroidSize size)
    {
        // Stores the prefab that matches the requested asteroid size
        GameObject asteroidPrefab = null;

        // Selects the correct prefab based on the size parameter.
        switch (size)
        {
            case Asteroid.AsteroidSize.Large:
                asteroidPrefab = largeAsteroidPrefab;
                break;
            
            case Asteroid.AsteroidSize.Medium:
                asteroidPrefab = mediumAsteroidPrefab;
                break;
            
            case Asteroid.AsteroidSize.Small:
                asteroidPrefab = smallAsteroidPrefab;
                break;
        }
        // Checks whether the required asteroid prefab was assigned.
        if(asteroidPrefab == null)
        {
            //Prints a warning for which prefab is missing.
            Debug.LogWarning(size + " asteroid prefab has not been assigned.");
            return;
        }
        // Creates the selected asteroid at the requested position.
        Instantiate(asteroidPrefab, position, Quaternion.identity);
    }
}