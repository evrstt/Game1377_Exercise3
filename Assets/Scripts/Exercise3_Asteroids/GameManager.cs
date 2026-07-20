using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set;}

    [SerializeField] private SpaceshipController player;
    [SerializeField] private int startingLives = 3;

    private int currentLives;

    public int CurrentLives
    {
        get
        {
            return currentLives;
        }
    }

    public SpaceshipController Player
    {
        get
        {
            return player;
        }
    }

    public Vector3 RespawnPosition
    {
        get
        {
            return Vector3.zero;
        }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        currentLives = startingLives;   
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    public bool LoseLife()
    {
        currentLives--;
        return currentLives > 0;
    }
}
