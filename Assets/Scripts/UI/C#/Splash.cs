using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Splash : MonoBehaviour
{
    public UIDocument _splashScreen;
    private static PlayerControl _playerControl;


    private void Awake()
    {
        _splashScreen = GetComponent<UIDocument>();
        _playerControl = GameObject.Find("Player")?.GetComponent<PlayerControl>();

    }
    void Start()
    {
        if (_playerControl == null)
        {
            Debug.LogError("Splash: PlayerControl not found.");
        }
        AudioManager._instance.playMusic("shrine");
    }

    public void StartGame()
    {
        int targetLevel = _playerControl._level == 0 ? 1 : _playerControl._level;
        SceneManager.LoadScene(targetLevel);
        SetMenuVisibility(false);
    }
    public void SetMenuVisibility(bool isVisible)
    {
        _splashScreen.rootVisualElement.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
    }
    public bool IsVisible()
    {
        return _splashScreen.rootVisualElement.style.visibility == Visibility.Visible;
    }

}
