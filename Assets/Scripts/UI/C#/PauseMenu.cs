using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    #region Initialization

    public UIDocument _pauseMenu;
    private VisualElement _bgPause;
    private Button _continue;
    private Button _settingsPM;
    private Button _mainMenuPM;

    public bool _cont, _openSettings, _mainMenu = false;

    private void Awake()
    {
        _pauseMenu = GetComponent<UIDocument>();
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
            _continue.RegisterCallback<ClickEvent>((evt) => 
            { 
                _cont = true;
            });
            _continue.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _cont = true;
            });
            _continue.RegisterCallback<PointerLeaveEvent>((evt) => 
            { 
                _continue.Focus();
            });
            _settingsPM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _settingsPM.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _settingsPM.RegisterCallback<PointerLeaveEvent>((evt) => 
            { 
                _settingsPM.Focus();
            });
            _mainMenuPM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _mainMenu = true;
            });
            _mainMenuPM.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _mainMenu = true;
            });
            _mainMenuPM.RegisterCallback<PointerLeaveEvent>((evt) => 
            { 
                _mainMenuPM.Focus();
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
