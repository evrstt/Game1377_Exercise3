using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PowerUps))]

public class SpaceshipController : MonoBehaviour
{
    private const string fireInput = "Fire";
    private const string hyperspaceInput = "Hyperspace";

    private const string deathAnimationTrigger = "Death";
    private const string fireAnimationTrigger = "Fire";
    private const string hyperspaceAnimationTrigger = "Hyperspace";
    private const string thrustAnimationBool = "IsThrusting";

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private Animator thrustAnimator;
    [SerializeField] private Animator shieldAnimator;
    [SerializeField] private Animator hyperspaceAnimator;
    
    [SerializeField] private AudioSource effectsAudioSource;
    [SerializeField] private AudioSource thrustAudioSource;
    [SerializeField] private AudioClip fireAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip hyperspaceAudioClip;
    [SerializeField] private AudioClip thrustAudioClip;

    [SerializeField] private PowerUps powerUps;

    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float thrustForce = 500f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 0.25f;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private float spawnShieldDuration = 3f;
    [SerializeField] private float hyperspaceSafeRadius = 2f;
    [SerializeField] private int maxHyperspaceAttempts = 25;
    [SerializeField] private LayerMask asteroidLayer;

    private float rotationInput;
    private float thrustInput;
    private float nextFireTime;
    private float defaultSpeedMultiplier = 1f;
    private bool canControl = true;
    private bool isRespawning;
    private bool isInvincible;

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

        if(effectsAudioSource = null)
        {
            effectsAudioSource = GetComponent<AudioSource>();
        }

        if(powerUps == null)
        {
            powerUps = GetComponent<PowerUps>();
        }

        if(weaponAnimator == null)
        {
            Debug.LogWarning("Weapon Animator has not been assigned.");
        }

        if(thrustAnimator == null)
        {
            Debug.LogWarning("Thrust Animator has not been assigned.");
        }

        if(shieldAnimator == null)
        {
            Debug.LogWarning("Shield Animator has not been assigned.");
        }
    }

   
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

    private void FixedUpdate()
    {
        if(canControl)
        {
            HandleThrust();
        }
    }

    private void HandleRotation()
    {
        float rotationAmount = -rotationInput * rotationSpeed * GetSpeedMultiplier() * Time.deltaTime;
        transform.Rotate(Vector3.forward * rotationAmount);
    }

    private void HandleThrust()
    {
        if(thrustInput > 0f)
        {
            rb.AddForce(transform.up * thrustForce * GetSpeedMultiplier() * thrustInput * Time.deltaTime);
        }
    }

    private float GetSpeedMultiplier()
    {
        if(powerUps != null)
        {
            return powerUps.SpeedMultiplier;
        }
        return defaultSpeedMultiplier;
    }

    private void HandleThrustEffects()
    {
        bool isThrusting = thrustInput > 0f;

        if(thrustAnimator != null)
        {
            thrustAnimator.SetBool(thrustAnimationBool, isThrusting);
        }

        if(thrustAudioSource == null || thrustAudioClip == null)
        {
            return;
        }

        if(isThrusting && !thrustAudioSource.isPlaying)
        {
            thrustAudioSource.clip = thrustAudioClip;
            thrustAudioSource.loop = true;
            thrustAudioSource.Play();
        }
        else if(!isThrusting && thrustAudioSource.isPlaying)
        {
            thrustAudioSource.Stop();
        }
    }

    private void StopThrustEffects()
    {
        if(thrustAnimator != null)
        {
            thrustAnimator.SetBool(thrustAnimationBool, false);
        }

        if(thrustAudioSource != null && thrustAudioSource.isPlaying)
        {
            thrustAudioSource.Stop();
        }
    }

    private void HandleFire()
    {
        if(Input.GetButtonDown(fireInput) && Time.time >= nextFireTime)
        {
            FireBullet();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    private void FireBullet()
    {
        if(bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab has not been assigned.");
            return;
        }
        
        if(firePoint == null)
        {
            Debug.LogWarning("Fire Point has not been assigned.");
            return;
        }

        GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if(powerUps != null)
        {
            newBullet.transform.localScale *= powerUps.BulletSizeMultiplier;
        }

        if(weaponAnimator != null)
        {
            weaponAnimator.SetTrigger(fireAnimationTrigger);
        }

        PlaySound(fireAudioClip);
    }

    private void HandleHyperspace()
    {
        if(!Input.GetButtonDown(hyperspaceInput))
        {
            return;
        }

        Vector3 safePostion;

        if(FindSafePosition(out safePostion))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            if(hyperspaceAnimator != null)
            {
                hyperspaceAnimator.SetTrigger(hyperspaceAnimationTrigger);
            }
        
        PlaySound(hyperspaceAudioClip);
        transform.position = safePostion;
        }

        else
        {
            Debug.LogWarning("A Safe hyperspace posotion could not be found");
        }
    }

    private bool FindSafePosition(out Vector3 safePosition)
    {
        for(int attempt = 0; attempt < maxHyperspaceAttempts; attempt++)
        {
            float randomX = Random.Range(ScreenBounds.screenLeft, ScreenBounds.screenRight);
            float randomY = Random.Range(ScreenBounds.screenTop, ScreenBounds.screenBottom);

            Vector3 possiblePositions = new Vector3(randomX, randomY, transform.position.z);

            Collider2D nearbyAsteroid = Physics2D.OverlapCircle(possiblePositions, hyperspaceSafeRadius, asteroidLayer);

            if(nearbyAsteroid == null)
            {
                safePosition = possiblePositions;
                return true;
            }
        }

        safePosition = transform.position;
        return false;
    }

    private void HandleDeath()
    {
        if(isRespawning || isInvincible)
        {
            return;
        }

        isRespawning = true;

        bool hasLivesRemaining = false;
        
        if(GameManager.Instance != null)
        {
            hasLivesRemaining = GameManager.Instance.LoseLife();
        }

        else
        {
            Debug.LogWarning("A GameManager Instance could not be found.");
        }

        StartCoroutine(DeathAndRespawnRoutine(hasLivesRemaining));
    }

    private IEnumerator DeathAndRespawnRoutine(bool hasLivesRemaining)
    {
        canControl = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if(playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        StopThrustEffects();

        if(thrustAnimator != null)
        {
            thrustAnimator.SetTrigger(deathAnimationTrigger);
        }

        PlaySound(deathAudioClip);

        yield return new WaitForSeconds(respawnDelay);

        if(!hasLivesRemaining)
        {
            gameObject.SetActive(false);
            yield break;
        }

        if (GameManager.Instance != null)
        {
            transform.position = GameManager.Instance.RespawnPosition;
        }

        else
        {
            transform.position = Vector3.zero;
        }

        transform.rotation = Quaternion.identity;

        isInvincible = true;
        canControl = true;

        yield return new WaitForSeconds(spawnShieldDuration);

        if(playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        isInvincible = false;
        isRespawning = false;
    }

    private void PlaySound(AudioClip audioClip)
    {
        if(effectsAudioSource != null && audioClip != null)
        {
            effectsAudioSource.PlayOneShot(audioClip);
        }
    }
    
}
