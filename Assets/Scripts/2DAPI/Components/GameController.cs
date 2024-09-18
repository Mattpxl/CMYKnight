using UnityEngine.EventSystems;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController _instance;
    public EventSystem _eventSystem;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Apply DontDestroyOnLoad to the entire GameController

            // Find the EventSystem in the child hierarchy
            _eventSystem = GetComponentInChildren<EventSystem>();

            if (_eventSystem == null)
            {
                Debug.LogError("No EventSystem found as a child of GameController.");
            }
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameController objects
        }
    }
    void Start()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

        if (eventSystems.Length > 1)
        {
            // Destroy any extra EventSystem instances
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
    }
    void DestroyPlayer()
    {
    // Optionally reset the game state or clean up objects
        var player = FindObjectOfType<PlayerControl>();
        if (player != null)
        {
        Destroy(player.gameObject); // Destroy player instance or reset game state
        }
    }
}
