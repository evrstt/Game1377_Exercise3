using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    // Makes sure the spaceship has a Rigidbody2D component
    [SerializeField] private Rigidbody2D rb;

    // Controls how quickly the spaceship rotates.
    [SerializeField] private float rotationSpeed = 360f;

    // Controls how much forward force is applied to the spaceship.
    [SerializeField] private float thrustForce = 500f;

    // Position and rotation where bullets will be created.
    [SerializeField] private Transform firePoint;

    // Bullet prefab that will be created when the player fires.
    [SerializeField] private GameObject bulletPrefab;

    // Stores the player's horizontal input for rotation
    private float rotationInput;

    // Stores the players vertical Input for forward thrust
    private float thrustInput;

    void Start()
    {
        // Gets the Rigidbody2D attached to the spaceship.
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rotationInput = Input.GetAxis("Horizontal");
        thrustInput = Input.GetAxis("Vertical");
        HandleRotation();
        HandleFire();
        HandleHyperspace();
    }

    void FixedUpdate()
    {
        HandleThrust();
    }

    private void HandleRotation()
    {
        //The negative input makes A rotate counterclockwise
        // and D rotate clockwise, to simulate space/flight controls.
        float rotationAmount = -rotationInput * rotationSpeed * Time.deltaTime;
        
        // rotates the spaceship around its Z axis
        transform.Rotate(Vector3.forward * rotationAmount); 
    }

    private void HandleThrust()
    {
        // Only applies thrust when the verticle Input is positive
        // This allows W to move the ship forword whilst ignoring S input.
        if (thrustInput > 0f)
        {
            //Applies force in the direciton the ship is facing.
            rb.AddForce(transform.up * thrustForce * thrustInput * Time.fixedDeltaTime);
        }
    }

    private void HandleFire()
    {
        // Fires one bullet when Fire1 input is pressed.
        if(Input.GetButtonDown("Fire1"))
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        // Prints an error if no bullet prefab has been assigned
        // and stops the method before it can Instantiate() the missing prefab
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not assigned!");
            return;
        }
        // Creates a bullet from the fire point using the fire points rotation.
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    private void HandleHyperspace()
    {
        // Teleports the ship to a random location whe left shift is pressed.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            TeleportToRandomLocation();
        }
    }

    private void TeleportToRandomLocation()
    {
        // Selects a random X position between the left and right edges of the screen.
        float randomX = Random.Range(ScreenBounds.screenLeft, ScreenBounds.screenRight);
        // Selects a random Y location between the top and bottom  edges of the screen.
        float randomY = Random.Range(ScreenBounds.screenBottom, ScreenBounds.screenTop);

        // Moves the spaceship to the random position
        transform.position = new Vector3(randomX, randomY, transform.position.z); 
    }
}
