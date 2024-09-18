using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    #region Initialization
    public UIDocument _mainMenu;
    public Button _start;
    public Button _settingsMM;
    public Button _quitMM;
    private AudioSource _audioSource;

    public bool isOpenSettings, isQuit, isCanStart = false;

    private void Awake()
    {
        _mainMenu = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _start = _mainMenu.rootVisualElement.Q("btnStart") as Button;
        _settingsMM = _mainMenu.rootVisualElement.Q("btnSettingsMM") as Button;
        _quitMM = _mainMenu.rootVisualElement.Q("btnQuitMM") as Button;

        // Add null checks
        if (_start == null || _settingsMM == null || _quitMM == null)
        {
            Debug.LogError("One or more buttons are missing from the UI.");
        }
    }

    #endregion Initialization
    void Start()
    {
        SetMenuVisibility(true);
        _start.Focus();
        InitCallbacks();
    }

    public void InitCallbacks()
    {
        RegisterButtonCallbacks(_start, () => isCanStart = true);
        RegisterButtonCallbacks(_settingsMM, () => isOpenSettings = true);
        RegisterButtonCallbacks(_quitMM, () => isQuit = true);
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
        _mainMenu.rootVisualElement.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        if (isVisible)
        {
            _start.Focus();
            isOpenSettings = false;
            isQuit = false;
            isCanStart = false;
        }
    }
    public bool IsVisible()
    {
        return _mainMenu.rootVisualElement.style.visibility == Visibility.Visible;
    }
}
