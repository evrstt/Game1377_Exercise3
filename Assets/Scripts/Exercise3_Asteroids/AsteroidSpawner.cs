using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    // Stores the asteroid prefabs in enum
    // Element 0 = Small
    // Element 1 = Medium
    // Element 2 = Large
    [SerializeField] private Asteroid[] asteroidPrefabs = new Asteroid[3];

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

    /// <summary>
    /// Calculates the screen boundaries and creates the initial asteroids
    /// </summary>
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

    /// <summary>
    /// Creates the starting large asteroids at random safe positions
    /// </summary>
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

    /// <summary>
    /// Creates asteroids of requested size and position
    /// </summary>
    /// <param name="position">
    /// The World position where the asteroid will be created
    /// </param>
    /// <param name="size">
    /// the size of asteroid that will be created
    /// </param>

    public void SpawnAsteroid(Vector3 position, Asteroid.AsteroidSize size)
    {
        // Converts enum into an interger array index
       int prefabIndex = (int)size;

        // Checks whether the array contains the requested index
       if(prefabIndex < 0 || prefabIndex >= asteroidPrefabs.Length)
        {
            Debug.LogWarning("The requested asteroid size does not exist in the prefab array");
            return;
        }

        // Gets the enum's numerical value
        Asteroid asteroidPrefab = asteroidPrefabs[prefabIndex];

        // Checks whether a prefab has been assigned to this array element
        if(asteroidPrefab == null)
        {
            Debug.LogWarning(size + " asteroid prefab has not been assigned.");
            return;
        }
        // Creates the asteroid and stores its Asteroid component
        Asteroid newAsteroid = Instantiate(asteroidPrefab, position, Quaternion.identity);
        // Gives the new asteroid a refernence to this spawner and tells it what size it is
        newAsteroid.SetSpawner(this);
        newAsteroid.SetSize(size);
    }
}