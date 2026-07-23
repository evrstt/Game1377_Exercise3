using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]

public class SpaceshipControllerOld : MonoBehaviour
{
    private const string fireInput = "Fire";
    private const string hyperspaceInput = "Hyperspace";

    // Stores the names of the Animator parameters
    private const string deathAnimationTrigger = "Death";
    private const string fireAnimationTrigger = "Fire";
    private const string hyperspaceAnimationTrigger = "Hyperspace";
    private const string thrustAnimationBool = "IsThrusting";

    // Makes sure the spaceship has a Rigidbody2D component
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource effectsAudio;
    [SerializeField] private AudioSource thrustAudio;

    // Controls how quickly the spaceship rotates.
    [SerializeField] private float rotationSpeed = 360f;

    // Controls how much forward force is applied to the spaceship.
    [SerializeField] private float thrustForce = 500f;

    // Position and rotation where bullets will be created.
    [SerializeField] private Transform firePoint;

    // Bullet prefab that will be created when the player fires.
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 0.25f;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float InvincibilityDuration = 3f;
    [SerializeField] private float hperspaceSafeRadius = 2f;
    [SerializeField] private int maxHyperspaceAttempts = 25;
    [SerializeField] private LayerMask asteroidLayer;
    [SerializeField] private AudioClip fireAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip hyperSpaceAudioClip;
    [SerializeField] private AudioClip thrustAudioClip;

    // Stores the player's horizontal input for rotation
    private float rotationInput;

    // Stores the players vertical Input for forward thrust
    private float thrustInput;

    private float nextFireTime;
    private bool canControl = true;
    private bool isRespawning;
    private bool isInvincible;
    private float baseRotationSpeed;
    private float baseThrustForce;

    private float bulletSizeMultiplier = 1f;
    private Coroutine speedBoost;
    private Coroutine bulletSizeBoost;

    void Start()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if(playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        if(animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if(effectsAudio == null)
        {
            effectsAudio = GetComponent<AudioSource>();
        }

        baseRotationSpeed = rotationSpeed;
        baseThrustForce = thrustForce;
    }

        /// < summary>
        /// Reads player Input and handles rotation firing and hyperspace
        /// </summary>
    void Update()
    {
        if(!canControl)
        {
            rotationInput = 0f;
            thrustInput = 0f;
            StopThrustEffects();
            return;
        }

        rotationInput = Input.GetAxis("Horizontal");
        thrustInput = Input.GetAxis("Vertical");
        HandleRotation();
        HandleFire();
        HandleHyperspace();
        HandleThrustEffects();
    }

    /// <summary>
    /// Handles physics-based spaceship thrust
    /// </summary>
    void FixedUpdate()
    {
        if(canControl)
        {
            HandleThrust();
        }
    }

    /// <summary>
    /// Rotates the spaceship using the players horizontal input
    /// </summary>
    private void HandleRotation()
    {
        //The negative input makes A rotate counterclockwise
        // and D rotate clockwise, to simulate space/flight controls.
        float rotationAmount = -rotationInput * rotationSpeed * Time.deltaTime;
        
        // rotates the spaceship around its Z axis
        transform.Rotate(Vector3.forward * rotationAmount); 
    }

    /// <summary>
    /// Applies forward force when vertical input is positive
    /// </summary>
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

    private void HandleThrustEffects()
    {
        bool isThrusting = thrustInput > 0f;

        if(animator != null)
        {
            animator.SetBool(thrustAnimationBool, isThrusting);
        }

        if(thrustAudio == null || thrustAudioClip == null)
        {
            return;
        }

        if(isThrusting && !thrustAudio.isPlaying)
        {
            thrustAudio.clip = thrustAudioClip;
            thrustAudio.loop = true;
            thrustAudio.Play();
        }

        else if(!isThrusting && thrustAudio.isPlaying)
        {
            thrustAudio.Stop();
        }
    }

    private void StopThrustEffects()
    {
        if(animator != null)
        {
            animator.SetBool(thrustAnimationBool, false);
        }

        if(thrustAudio != null && thrustAudio.isPlaying)
        {
            thrustAudio.Stop();
        }
    }
    /// <summary>
    /// Checks whether fire input button is pressed
    /// </summary>
    private void HandleFire()
    {
        // Fires one bullet when the assigned input is pressed (assigned to space).
        if(Input.GetButtonDown(fireInput) && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    /// <summary>
    /// Creates a fire point at the assigned fire point
    /// </summary>
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
        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        newBullet.transform.localScale *= bulletSizeMultiplier;

        if(animator != null)
        {
            animator.SetTrigger(fireAnimationTrigger);
        }

        PlaySound(fireAudioClip);
    }
   

    /// <summary>
    /// Checks whether the hyperspace input button was pressed
    /// </summary>
    private void HandleHyperspace()
    {
        // Teleports the ship to a random location whe left shift is pressed.
        if(!Input.GetButtonDown(hyperspaceInput))
        {
            return;
        }
    }

    /// <summary>
    /// Teleports the spaceship to a random positio within screen bounds
    /// </summary>
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
