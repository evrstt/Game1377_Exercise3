using UnityEngine;

// Ensures that any GameObject using this script has a Ridigbody2D component.
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    // Controls how quickly the bullet moves.
    [SerializeField] private float bulletSpeed = 20f;

    // Controls how long the bullet exists before being destroyed.
    [SerializeField] private float bulletLifetime = 5f;

    // Sores a reference to the bullet's Rigidbody2D component.
    private Rigidbody2D rb;

    void Start()
    {
        // Gets the Rigidbody2d attached to the bullet.
        rb =  GetComponent<Rigidbody2D>();

        // Moves the bullet forward in the direciton it is facing.
        rb.linearVelocity = transform.up * bulletSpeed;

        // Automatically destroys the bullet after its lifetime expires.
        Destroy(gameObject, bulletLifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks whether the object the bullet collided with has an Asteroid component
        if(collision.gameObject.GetComponent<Asteroid>() != null)
        {
            // Destroys the bullet after it his an asteroid.
            Destroy(gameObject);
        }
    }
}
