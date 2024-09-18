using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    public UIDocument _endscreen;
    private AudioSource _audioSource;
    private Button _restart;
    private Button _quit;

    public bool isQuit = false;
    public bool isRestart = false;

    void Awake()
    {
        _endscreen = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _restart = _endscreen.rootVisualElement.Q("btnRestart") as Button;
        _quit = _endscreen.rootVisualElement.Q("btnQuit") as Button;

        if (_restart == null || _quit == null)
        {
            Debug.LogError("EndScreen: One or both buttons are missing in the UI.");
        }
    }

    void Start()
    {
        SetMenuVisibility(false);
        InitCallbacks();
    }

    private void InitCallbacks()
    {
        RegisterButtonCallbacks(_quit, () => isQuit = true);
        RegisterButtonCallbacks(_restart, () => isRestart = true);
    }

    private void RegisterButtonCallbacks(Button button, System.Action onSubmitAction)
    {
        var sound = AudioManager._instance._sfxUI[6]._sound;

        button.RegisterCallback<ClickEvent>((evt) => onSubmitAction());
        button.RegisterCallback<NavigationSubmitEvent>((evt) => onSubmitAction());
        button.RegisterCallback<NavigationMoveEvent>((evt) => _audioSource.PlayOneShot(sound));
        button.RegisterCallback<PointerEnterEvent>((evt) =>
        {
            _audioSource.PlayOneShot(sound);
            button.Focus();
        });
    }

    public void SetMenuVisibility(bool isVisible)
    {
        _endscreen.rootVisualElement.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        if (isVisible)
        {
            isQuit = false;
            isRestart = false;
        }
    }
    public bool IsVisible()
    {
        return _endscreen.rootVisualElement.style.visibility == Visibility.Visible;
    }
}
