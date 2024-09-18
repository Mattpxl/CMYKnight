using UnityEngine;
using UnityEngine.EventSystems;
public class DontDestroy : MonoBehaviour
{
    public static DontDestroy _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameController objects
        }
    }
}
