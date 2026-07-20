using UnityEngine;

/// <summary>
/// Nabages sgared gane state such as the player reference
/// remaining lives and player respawn position
/// </summary>
public class GameManager : MonoBehaviour
{

    // Provides global access to the the GameManager
    public static GameManager Instance{get; private set;}

    // Stores the player ship.
    [SerializeField] private SpaceshipController player;

    // Controls how many lives the player has when the game starts
    [SerializeField] private int startingLives = 3;

    // Stores the current number of lives
    private int currentLives;
    
    // Allows other scripts to read the current number of lives
    public int CurrentLives
    {
        get
        {
            return currentLives;
        }
    }

    // Allows other scripts to access the player spaceship
    public SpaceshipController Player
    {
        get
        {
            return player;
        }
    }

    // returns the center of the game screen as the spawn position
    public Vector3 RespawnPosition
    {
        get
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Creates the Singleton instance and destroys duplicate GameManagers
    /// </summary>
    private void Awake()
    {
        // Checks whether another GameManager Instance already exists
        // and destroys this one if one does
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Stores this object as the singleton instance
        Instance = this;
    }

    /// <summary>
    /// Sets the players current lives to the configured start amount.
    /// </summary>
    void Start()
    {
        currentLives = startingLives;   
    }

    /// <summary>
    ///  Clears the singleton reference when the GameManager is destroyed
    /// </summary>
    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Removes one life from the player
    /// </summary>
    /// <returns>
    /// True when the player still has at least one life remaining
    /// </returns>
    public bool LoseLife()
    {
        currentLives--;
        return currentLives > 0;
    }

    /// <summary>
    /// Adds lives to the player's current life total.
    /// </summary>
    /// <param name="lifeAmount">
    /// the number of lives that should be added.
    /// </param>
    public void AddLife(int lifeAmount)
    {
        if(lifeAmount <= 0)
        {
            return;
        }

        currentLives += lifeAmount;
    }
}
