using UnityEngine;

// Ensures that any GameObject using this script has a Ridigbody2D component.
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    // Controls how quickly the bullet moves.
    [SerializeField] private float bulletSpeed = 20f;

    // Controls how long the bullet exists before being destroyed.
    [SerializeField] private float bulletLifetime = 5f;

    // Stores a reference to the bullet's Rigidbody2D component.
    private Rigidbody2D rb;

    // Stores the tag assigned assigned to asteroid objects
    private const string asteroidTag = "Asteroid";

    /// <summary>
    /// Gives bullet its starting velocity and begins its lifetime timer.
    /// </summary>
    void Start()
    {
        // Gets the Rigidbody2d attached to the bullet.
        rb =  GetComponent<Rigidbody2D>();

        // Moves the bullet forward in the direciton it is facing.
        rb.linearVelocity = transform.up * bulletSpeed;

        // Automatically destroys the bullet after its lifetime expires.
        Destroy(gameObject, bulletLifetime);
    }

    /// <summary>
    /// Destroys the bullet when it collides with an asteroid
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks whether the object the bullet collided with is tagged with an asteroid
        if(collision.gameObject.CompareTag(asteroidTag))
        {
            // Destroys the bullet after it his an asteroid.
            Destroy(gameObject);
        }
    }
}
