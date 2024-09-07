using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    #region Initialization
    public UIDocument _mainMenu;
    private Button _start;
    private Button _settingsMM;
    private Button _quitMM;

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
            _settingsMM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _openSettings = true;
            });
            _quitMM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _quit = true;
            });
    }

    public void isEnabled()
    {
       // _mainMenu.enabled = true;
       // this.gameObject.SetActive(true);
        _mainMenu.rootVisualElement.style.visibility = Visibility.Visible;
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
