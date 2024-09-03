using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DataStorage;
using Unity.VisualScripting;

public class StateMachine : MonoBehaviour
{
    #region Initialization
    [SerializeField] private PlayerControl _playerControl;
    [SerializeField] private LevelManager _levelManager;
    //private SceneManager _sceneManager;
    private JSONConverter _jsonConverter;

    private void Awake()
    {
        //_sceneManager = GetComponent<SceneManager>();
        _jsonConverter = new JSONConverter();
        // RuneTimeUI
        _heart = _runtimeUI.rootVisualElement.Q("veHeart");
        _key  = _runtimeUI.rootVisualElement.Q("veKey");
        _bgPause = _pauseMenu.rootVisualElement.Q("bgPause");
        _heartLabel = _bgPause.Q("txtHeart") as TextField;
        _keyLabel = _bgPause.Q("txtKey") as TextField;
        // PauseMenu
        _continue = _pauseMenu.rootVisualElement.Q("btnContinue") as Button;
        _settingsPM = _pauseMenu.rootVisualElement.Q("btnSettingsPM") as Button;
        _mainMenuPM = _pauseMenu.rootVisualElement.Q("btnMenuPM")  as Button;
        // End Screen 
        _restart = _endscreen.rootVisualElement.Q("btnRestart") as Button;
        _quit = _endscreen.rootVisualElement.Q("btnQuit") as Button;
        // Main Menu
        _start = _mainMenu.rootVisualElement.Q("btnStart") as Button;
        _settingsMM = _mainMenu.rootVisualElement.Q("btnSettingsMM") as Button;
        _quitMM = _mainMenu.rootVisualElement.Q("btnQuitMM") as Button;
        // Settings Menu
        _exitSM  = _settingsMenu.rootVisualElement.Q("btnExitSM") as Button;
        _mute = _settingsMenu.rootVisualElement.Q("tglMute") as Toggle;
        _sound = _settingsMenu.rootVisualElement.Q("sdrSound") as Slider;
        _music = _settingsMenu.rootVisualElement.Q("sdrMusic") as Slider;
        _fullscreen = _settingsMenu.rootVisualElement.Q("tglFullScreen") as Toggle;
        _resolution = _settingsMenu.rootVisualElement.Q("drpResolution") as PopupField<DropdownMenu>;
        _quality = _settingsMenu.rootVisualElement.Q("drpQuality") as PopupField<DropdownMenu>;  
    }

    private void Start()
    {
        _runtimeUI.gameObject.SetActive(true);
        _pauseMenu.gameObject.SetActive(false);
        _endscreen.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
        _settingsMenu.gameObject.SetActive(false);
        // get values from JSONConverter
    }

    private void Update(){
        // Setting Menu
        if(_settingsMenu.gameObject.activeInHierarchy){
        _exitSM.RegisterCallback<ClickEvent>(OnUIClick);
        _mute.RegisterCallback<ClickEvent>(OnUIClick);
        _sound.RegisterCallback<ClickEvent>(OnUIClick);
        _music.RegisterCallback<ClickEvent>(OnUIClick);
        _fullscreen.RegisterCallback<ClickEvent>(OnUIClick);
       // _resolution.RegisterCallback<ClickEvent>(OnUIClick);
       // _quality.RegisterCallback<ClickEvent>(OnUIClick);
        }
        //MainMenu
        else if(_mainMenu.gameObject.activeInHierarchy){
        _start.RegisterCallback<ClickEvent>(OnUIClick);
        _settingsMM.RegisterCallback<ClickEvent>(OnUIClick);
        _quitMM.RegisterCallback<ClickEvent>(OnUIClick);
        }
        // End Screen
        else if(_endscreen.gameObject.activeInHierarchy){
            _quit.RegisterCallback<ClickEvent>(OnUIClick);
            _restart.RegisterCallback<ClickEvent>(OnUIClick);
        }
        // PauseMenu
        else if(_pauseMenu.gameObject.activeInHierarchy){
            _continue.RegisterCallback<ClickEvent>(OnUIClick);
            _settingsPM.RegisterCallback<ClickEvent>(OnUIClick);
            _mainMenuPM.RegisterCallback<ClickEvent>(OnUIClick);
        }
    }

    private void OnEnable() 
    {
          // Setting Menu
        if(_settingsMenu.gameObject.activeInHierarchy){
        _exitSM.RegisterCallback<ClickEvent>(OnUIClick);
        _mute.RegisterCallback<ClickEvent>(OnUIClick);
        _sound.RegisterCallback<ClickEvent>(OnUIClick);
        _music.RegisterCallback<ClickEvent>(OnUIClick);
        _fullscreen.RegisterCallback<ClickEvent>(OnUIClick);
       // _resolution.RegisterCallback<ClickEvent>(OnUIClick);
       // _quality.RegisterCallback<ClickEvent>(OnUIClick);
        }
        //MainMenu
        else if(_mainMenu.gameObject.activeInHierarchy){
        _start.RegisterCallback<ClickEvent>(OnUIClick);
        _settingsMM.RegisterCallback<ClickEvent>(OnUIClick);
        _quitMM.RegisterCallback<ClickEvent>(OnUIClick);
        }
        // End Screen
        else if(_endscreen.gameObject.activeInHierarchy){
            _quit.RegisterCallback<ClickEvent>(OnUIClick);
            _restart.RegisterCallback<ClickEvent>(OnUIClick);
        }
        // PauseMenu
        else if(_pauseMenu.gameObject.activeInHierarchy){
            _continue.RegisterCallback<ClickEvent>(OnUIClick);
            _settingsPM.RegisterCallback<ClickEvent>(OnUIClick);
            _mainMenuPM.RegisterCallback<ClickEvent>(OnUIClick);
        }
    }

    private void OnApplicationQuit()
    {
        // save settings values and json values from player data
    }
    #endregion Initialization

    #region Input

    private void OnUIClick(ClickEvent evt)
    {
        // Pause Menu
        if(evt.currentTarget == _continue)
        {
            _pauseMenu.gameObject.SetActive(false);
            // !_isPaused
        }
        else if (evt.currentTarget == _settingsPM)
        {
            _settingsMenu.gameObject.SetActive(true);
        }
        else if (evt.currentTarget == _mainMenuPM)
        {
            _mainMenu.gameObject.SetActive(true);
            _pauseMenu.gameObject.SetActive(false);
            // are you sure? You Will Have To Restart This World Menu
            // save data
        }
        // End Screen
        if(evt.currentTarget == _quit)
        {
            _mainMenu.gameObject.SetActive(true);
            _endscreen.gameObject.SetActive(false);
            // are you sure? You Will Have To Restart This World Menu
        }
        else if (evt.currentTarget == _restart)
        {
            _endscreen.gameObject.SetActive(false);
            // rspawn player
            // unpause
        }
        // Main Menu
        if(evt.currentTarget == _start)
        {
            _mainMenu.gameObject.SetActive(false);
            // unpause
            // spawn player
        }
        else if (evt.currentTarget == _settingsMM)
        {
            _settingsMenu.gameObject.SetActive(true);
        }
        else if (evt.currentTarget == _quitMM)
        {
            // return to splash screen scene
        }
        // Settings Menu
        if(evt.currentTarget == _exitSM)
        {
            _settingsMenu.gameObject.SetActive(false);
        }
        if(evt.currentTarget == _mute)
        {
            
        }
        else if(evt.currentTarget == _sound)
        {

        }
        else if(evt.currentTarget == _music)
        {

        }
        else if(evt.currentTarget == _fullscreen)
        {

        }
        else if(evt.currentTarget == _resolution)
        {

        }
        else if(evt.currentTarget == _quality)
        {

        }
    }
    #endregion Input

    #region Functionality

    // create OnPause Function to pause everything

    #endregion functionality

    #region RuntimeUI

    [SerializeField] private UIDocument _runtimeUI;
    private VisualElement _heart;
    private VisualElement _key;
    private TextField _heartLabel;
    private TextField _keyLabel;

    #endregion RuntimeUI

    #region PauseMenu
    [SerializeField] private UIDocument _pauseMenu;
    private VisualElement _bgPause;
    private Button _continue;
    private Button _settingsPM;
    private Button _mainMenuPM;

    #endregion PauseMenu

    #region EndScreen
    [SerializeField] private UIDocument _endscreen;
    private Button _restart;
    private Button _quit;

    #endregion EndScreen

    #region MainMenu
    [SerializeField] private UIDocument _mainMenu;
    private Button _start;
    private Button _settingsMM;
    private Button _quitMM;

    #endregion MainMenu

    #region SettingMenu
    [SerializeField] private UIDocument _settingsMenu;
    private VisualElement _settingsPanel;
    private Button _exitSM;
    private Toggle _mute;
    private Slider _sound;
    private Slider _music;
    private Toggle _fullscreen;
    private PopupField<DropdownMenu> _resolution;
    private PopupField<DropdownMenu>  _quality;
    private Resolution[] _resolutions;

    #endregion SettingsMenu
}
