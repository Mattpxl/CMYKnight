using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    #region Initialization

    public UIDocument _pauseMenu;
    private AudioSource _audioSource;
    private VisualElement _bgPause;
    private Button _continue;
    private Button _settingsPM;
    private Button _mainMenuPM;

    public bool isContinue = false;
    public bool isOpenSettings = false;
    public bool isMainMenu = false;

    private void Awake()
    {
        _pauseMenu = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _bgPause = _pauseMenu.rootVisualElement.Q("bgPause");
        _continue = _pauseMenu.rootVisualElement.Q("btnContinue") as Button;
        _settingsPM = _pauseMenu.rootVisualElement.Q("btnSettingsPM") as Button;
        _mainMenuPM = _pauseMenu.rootVisualElement.Q("btnMenuPM") as Button;

        if (_continue == null || _settingsPM == null || _mainMenuPM == null)
        {
            Debug.LogError("PauseMenu: One or more buttons are missing in the UI.");
        }
    }
    #endregion Initialization

    void Start()
    {
        InitCallbacks();
        SetMenuVisibility(false);
    }

    private void InitCallbacks()
    {
        RegisterButtonCallbacks(_continue, () => isContinue = true);
        RegisterButtonCallbacks(_settingsPM, () => isOpenSettings = true);
        RegisterButtonCallbacks(_mainMenuPM, () => isMainMenu = true);
    }

    private void RegisterButtonCallbacks(Button button, System.Action onSubmitAction)
    {
        var sound = AudioManager._instance._sfxUI[6]._sound;

        button.RegisterCallback<ClickEvent>((evt) => onSubmitAction());
        button.RegisterCallback<NavigationSubmitEvent>((evt) => onSubmitAction());
        button.RegisterCallback<PointerEnterEvent>((evt) =>
        {
            _audioSource.PlayOneShot(sound);
            button.Focus();
        });
        button.RegisterCallback<NavigationMoveEvent>((evt) => _audioSource.PlayOneShot(sound));
    }

    public void SetMenuVisibility(bool isVisible)
    {
        _pauseMenu.rootVisualElement.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        if (isVisible)
        {
            _continue.Focus();
            isContinue = false;
            isOpenSettings = false;
            isMainMenu = false;
        }
    }
    public bool IsVisible()
    {
        return _pauseMenu.rootVisualElement.style.visibility == Visibility.Visible;
    }
}
