using UnityEngine;

// Ensures the asteroids will have a Rigidbody2D component.
[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
    // List of sizes an asteroid can have.
    public enum AsteroidSize { Small, Medium, Large }

    // Stores tags for bullet and player for collision.
    private const string bulletTag = "Bullet";
    private const string playerTag = "Player";

    // Stores the current size of the asteroid
    [SerializeField] private AsteroidSize size;

    // Controls the movement speed
    [SerializeField] private float speed;

    // Stores the min/max possible rotation speeds
    [SerializeField] private float minRotationSpeed = -180f;
    [SerializeField] private float maxRotationSpeed = 180f;

    // Controls how many smaller asteroids are created.
    [SerializeField] private int childAsteroidCount = 2;

    // Stores the asteroids Rigidbody2D
    private Rigidbody2D rb;

    // Stores a reference to the Asteroid spawner.
    private AsteroidSpawner spawner;

    // Stores the asteroid's randomly selected velocity.
    private Vector2 velocity;

    void Start()
    {
        // Gets the Rigidbody2d attached to the asteroid.
        rb = GetComponent<Rigidbody2D>();

        // Selects a random direction for the asteroid.
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Creates velocity in that random direction and applies it to the asteroids Rigidbody2D
        velocity = randomDirection * speed;
        rb.linearVelocity = velocity;
        rb.angularVelocity = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void Update()
    {
    }

    /// <summary>
    /// Stores the AsteroidSpawner that created this asteroid
    /// </summary>
    /// <param name="asteroidSpawner"></param>
    public void SetSpawner(AsteroidSpawner asteroidSpawner)
    {
        // Stores the Asteroid spawner that created the asteroid
        spawner = asteroidSpawner;
    }

    /// <summary>
    ///  Setes the current size of the asteroid
    /// </summary>
    /// <param name="asteroidSize"></param>
    public void SetSize(AsteroidSize asteroidSize)
    {
        // Sets the size of the asteroid
        size = asteroidSize;
    }

    /// <summary>
    /// Handles the Collision when a bullet or player hits the asteroid
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks whether the asteroid collided with a bullet
        if(collision.gameObject.CompareTag(bulletTag))
        {
            // Breaks or destroys asteroid
            BreakAsteroid();
        }
        // Checks if the asteroid collided with the player.
        else if(collision.gameObject.CompareTag(playerTag))
        {
            // destroys the players ship
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// Creates smaller asteroids when possible and destroys this asteroid
    /// </summary>
    private void BreakAsteroid()
    {
        if(size != AsteroidSize.Small)
        {
            AsteroidSize childSize = (AsteroidSize)((int)size - 1);
            SpawnChildren(childSize);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Creates the smaller asteroids
    /// </summary>
    /// <param name="childSize"></param>
    private void SpawnChildren(AsteroidSize childSize)
    {
        // Checks if the asteroid has a reference to the spawner.
        if(spawner == null)
        {
            Debug.LogWarning("The asteroid does not have an AsteroidSpawner reference");
            return;
        }

        // Creates two smaller asteroids from the destroyed asteroids location.
        for(int i = 0; i < childAsteroidCount; i++)
        {
            spawner.SpawnAsteroid(transform.position, childSize);
        }
    }
}
