using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    #region Initialization
    public UIDocument _mainMenu;
    public Button _start;
    public Button _settingsMM;
    public Button _quitMM;

    public bool _openSettings, _quit, _canStart = false;
    private void Awake()
    {
        _mainMenu = GetComponent<UIDocument>();
        _start = _mainMenu.rootVisualElement.Q("btnStart") as Button;
        _settingsMM = _mainMenu.rootVisualElement.Q("btnSettingsMM") as Button;
        _quitMM = _mainMenu.rootVisualElement.Q("btnQuitMM") as Button;
    }

    #endregion Initialization
    void Start()
    {
        isEnabled();
        _start.Focus();
        initCallbacks();
    }
    private void OnEnable()
    {
        initCallbacks();
    }

    public void initCallbacks()
    {
            _start.RegisterCallback<ClickEvent>((evt) => 
            { 
                _canStart = true;
            });
            _start.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _canStart = true;
            });
            _start.RegisterCallback<PointerLeaveEvent>((evt) => 
            { 
                _start.Focus();
            });
            _settingsMM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _settingsMM.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _settingsMM.RegisterCallback<PointerLeaveEvent>((evt) => 
            { 
                _settingsMM.Focus();
            });
            _quitMM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _quit = true;
            });
            _quitMM.RegisterCallback<NavigationSubmitEvent>((evt) => 
            { 
                _quit = true;
            });
            _quitMM.RegisterCallback<PointerLeaveEvent>((evt) => 
            { 
                _quitMM.Focus();
            });
    }

    public void isEnabled()
    {
       // _mainMenu.enabled = true;
       // this.gameObject.SetActive(true);
        _mainMenu.rootVisualElement.style.visibility = Visibility.Visible;
        _start.Focus();
       // initCallbacks();
        _openSettings = false;
        _quit = false;
        _canStart = false;
    }
    public void isDisabled()
    {
       
        _mainMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        //_mainMenu.enabled = false;
        //this.gameObject.SetActive(false);
        
    }
}
