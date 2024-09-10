using UnityEngine;
using UnityEngine.EventSystems;
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

    public bool _cont = false, _openSettings = false, _mainMenu = false;

    private void Awake()
    {
        _pauseMenu = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _bgPause = _pauseMenu.rootVisualElement.Q("bgPause");
        _continue = _pauseMenu.rootVisualElement.Q("btnContinue") as Button;
        _settingsPM = _pauseMenu.rootVisualElement.Q("btnSettingsPM") as Button;
        _mainMenuPM = _pauseMenu.rootVisualElement.Q("btnMenuPM")  as Button;
    }
    #endregion Initialization
    void Start()
    {
        initCallbacks();
        isDisabled();
    }
    void OnEnable()
    {
        initCallbacks();
    }
    public void initCallbacks()
    {
        // add PointerLeaveEvent, PointerOverEvent, NavigationMoveEvent for each.
            // continue
            _continue.RegisterCallback<ClickEvent>((evt) => 
            { 
                _cont = true;
            });
            _continue.RegisterCallback<PointerEnterEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
                _continue.Focus();
            });
            _continue.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _cont = true;
            });
            _continue.RegisterCallback<NavigationMoveEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            });
            _settingsPM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _settingsPM.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _settingsPM.RegisterCallback<NavigationMoveEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            });
            _settingsPM.RegisterCallback<PointerEnterEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
                _settingsPM.Focus();
            });
            // main menu
            _mainMenuPM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _mainMenu = true;
            });
            _mainMenuPM.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _mainMenu = true;
            });
            _mainMenuPM.RegisterCallback<PointerEnterEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
                _mainMenuPM.Focus();
            });
            _mainMenuPM.RegisterCallback<NavigationMoveEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            });
    }
    public void isEnabled()
    {
       // _pauseMenu.enabled = true;
       // this.gameObject.SetActive(true);
        _pauseMenu.rootVisualElement.style.visibility = Visibility.Visible;
        _continue.Focus();
        //initCallbacks();
        _cont = false;
        _openSettings = false;
        _mainMenu = false;
    }
    public void isDisabled()
    {
        if(_pauseMenu.isActiveAndEnabled)
        {
            _pauseMenu.rootVisualElement.style.visibility = Visibility.Hidden;
            //_pauseMenu.enabled = false;
            //this.gameObject.SetActive(false);
        }
    }
}
