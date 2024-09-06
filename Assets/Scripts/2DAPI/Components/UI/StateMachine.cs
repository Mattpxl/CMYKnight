using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using DataStorage;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class StateMachine : MonoBehaviour
{
    #region Initialization
    [SerializeField] private PlayerControl _playerControl;
    private bool _isPaused;
    private AudioSource _audioSource;

    private void Awake()
    {
        // RuneTimeUI
        _heart = _runtimeUI.rootVisualElement.Q("veHeart");
        _key  = _runtimeUI.rootVisualElement.Q("veKey");
        _heartLabel = _runtimeUI.rootVisualElement.Q("txtHeart") as TextField;
        _keyLabel = _runtimeUI.rootVisualElement.Q("txtKey") as TextField;
        // PauseMenu
        _bgPause = _pauseMenu.rootVisualElement.Q("bgPause");
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
        // Confirmation Menu
        _yes = _confirmationMenu.rootVisualElement.Q("btnYes") as Button;
        _no = _confirmationMenu.rootVisualElement.Q("btnNo") as Button;
        // Settings Menu
        _exitSM  = _settingsMenu.rootVisualElement.Q("btnExitSM") as Button;
        _mute = _settingsMenu.rootVisualElement.Q("tglMute") as Toggle;
        _sound = _settingsMenu.rootVisualElement.Q("sdrSound") as Slider;
        _music = _settingsMenu.rootVisualElement.Q("sdrMusic") as Slider;
        _fullscreen = _settingsMenu.rootVisualElement.Q("tglFullScreen") as Toggle;
        _resolution = _settingsMenu.rootVisualElement.Q("drpResolution") as DropdownField;

        _audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        _isPaused = _playerControl._isPaused;
        _runtimeUI.rootVisualElement.style.visibility = Visibility.Visible;
        _mainMenu.rootVisualElement.style.visibility = Visibility.Visible;
        _pauseMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        _endscreen.rootVisualElement.style.visibility = Visibility.Hidden;
        _confirmationMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        _settingsMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        _audioSource.Stop();
        AudioManager._instance._audioMixer.SetFloat("Sounds",0f);
        AudioManager._instance._musicSource.loop = true;
        AudioManager._instance.playMusic("menu0");
            // Setting Menu
            _exitSM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _settingsMenu.rootVisualElement.style.visibility = Visibility.Hidden;
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[3]._sound);
                // save settings data changes
            });

            // Confirmation menu
            _yes.RegisterCallback<ClickEvent>((evt) => 
            { 
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                AudioManager._instance.playMusic("menu0");
                _confirmationMenu.rootVisualElement.style.visibility = Visibility.Hidden;
                _mainMenu.rootVisualElement.style.visibility = Visibility.Visible;
                // Save
            });
            _no.RegisterCallback<ClickEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                _confirmationMenu.rootVisualElement.style.visibility = Visibility.Hidden;
            });
        
            //MainMenu
            _start.RegisterCallback<ClickEvent>((evt) => 
            { 
                AudioManager._instance._musicSource.Stop();
                AudioManager._instance._audioMixer.SetFloat("Sounds",0f);
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[4]._sound);
                AudioManager._instance.playMusic("cavern");
                _mainMenu.rootVisualElement.style.visibility = Visibility.Hidden;
                if (_isPaused == true)
                { 
                    _isPaused = false; 
                    _playerControl._isPaused = _isPaused;
                }
                _pauseMenu.rootVisualElement.style.visibility = Visibility.Hidden;
                _endscreen.rootVisualElement.style.visibility = Visibility.Hidden;
                _confirmationMenu.rootVisualElement.style.visibility = Visibility.Hidden;
                _settingsMenu.rootVisualElement.style.visibility = Visibility.Hidden;
                AudioManager._instance._audioMixer.SetFloat("Sounds",_sound.value);
                _playerControl.spawn();
            });
            _settingsMM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _settingsMenu.rootVisualElement.style.visibility = Visibility.Visible;
            });
            _quitMM.RegisterCallback<ClickEvent>((evt) => 
            { 
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
               SceneManager.LoadScene(0);
            });
        
            // End Screen
            _quit.RegisterCallback<ClickEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _confirmationMenu.rootVisualElement.style.visibility = Visibility.Visible;
            });
            _restart.RegisterCallback<ClickEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                AudioManager._instance._musicSource.Stop();
                AudioManager._instance.playMusic("cavern");
                _endscreen.rootVisualElement.style.visibility = Visibility.Hidden;
                if (_isPaused == true)
                { 
                    _isPaused = false; 
                    _playerControl._isPaused = _isPaused;
                }
                _playerControl.respawn();
            });
        
            // PauseMenu
            _continue.RegisterCallback<ClickEvent>((evt) => 
            { 
                // if pause menu visible
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[1]._sound);
                AudioManager._instance.playMusic("cavern");
                _pauseMenu.rootVisualElement.style.visibility = Visibility.Hidden; 
                if (_isPaused == true) 
                { 
                    _isPaused = false; 
                    _playerControl._isPaused = _isPaused;
                }
            });
            _settingsPM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _settingsMenu.rootVisualElement.style.visibility = Visibility.Visible;
            });
            _mainMenuPM.RegisterCallback<ClickEvent>((evt) => 
            { 
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _confirmationMenu.rootVisualElement.style.visibility = Visibility.Visible;
                _pauseMenu.rootVisualElement.style.visibility = Visibility.Hidden;
            });
            // Settings Menu Values
            _mute.value = Convert.ToBoolean((int)JSONConverter.jsonConverter.getDataObject("mute"));
            _mute.RegisterValueChangedCallback((evt) =>
            {
                AudioManager._instance.toggleMusic();
                _audioSource.mute = _mute.value? true : !_audioSource.mute;
                if(_audioSource.mute == true)
                {
                    AudioManager._instance._audioMixer.SetFloat("Music",0f);
                    AudioManager._instance._audioMixer.SetFloat("Sounds",0f);
                    AudioManager._instance._audioMixer.SetFloat("UI",0f);
                }
                else
                {
                    AudioManager._instance._audioMixer.SetFloat("Music",_music.value);
                    AudioManager._instance._audioMixer.SetFloat("Sounds",_sound.value);
                    AudioManager._instance._audioMixer.SetFloat("UI",_sound.value);
                }
            });
            _sound.value = JSONConverter.jsonConverter.getDataObject("sound");
            _sound.RegisterValueChangedCallback((evt) => 
            {
                AudioManager._instance._audioMixer.SetFloat("Sounds",_sound.value);
                AudioManager._instance._audioMixer.SetFloat("UI",_sound.value);
            });
            _music.value = JSONConverter.jsonConverter.getDataObject("music");
            _music.RegisterValueChangedCallback((evt) => 
            {
                AudioManager._instance._audioMixer.SetFloat("Music",_music.value);
            });
            _fullscreen.value = Convert.ToBoolean((int)JSONConverter.jsonConverter.getDataObject("fullscreen"));
            _fullscreen.RegisterValueChangedCallback((evt) => 
            {
                Screen.fullScreen = _fullscreen.value;
            });
            _resolutions = Screen.resolutions;
            _widths = new int[_resolutions.Length];
            _heights = new int[_resolutions.Length];

            for(int i = 0; i < _resolutions.Length; i++)
            {
            _resolution.choices.Add(_resolutions[i].width + " x " + _resolutions[i].height);
            _widths[i] = _resolutions[i].width;
            _heights[i] = _resolutions[i].height;
            if
            (
                _resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height
            )
            {
                _resIndex = i;
            }
            }
            _resolution.value = _resolution.choices[Convert.ToInt32(JSONConverter.jsonConverter.getDataObject("resolution"))];
            _resolution.RegisterValueChangedCallback((evt) => 
            {
                Screen.SetResolution(_widths[_resolution.index],_heights[_resolution.index], Screen.fullScreen);
                _resolution.value = _resolution.choices[_resolution.index];
                _saveIndex = _resolution.index;
            });
            if(_mute.value == true)
                {
                    AudioManager._instance._audioMixer.SetFloat("Music",0f);
                    AudioManager._instance._audioMixer.SetFloat("UI",0f);
                }
                else
                {
                    AudioManager._instance._audioMixer.SetFloat("Music",_music.value);
                    AudioManager._instance._audioMixer.SetFloat("UI",_sound.value);
                }

            if (_settingsMenu.rootVisualElement.style.visibility == Visibility.Visible){
            EventSystem.SetUITookitEventSystemOverride(null, true, false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Settings Menu"));
            _exitSM.Focus();
        }
        else if (_confirmationMenu.rootVisualElement.style.visibility == Visibility.Visible){
            EventSystem.SetUITookitEventSystemOverride(null, true, false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("ConfirmationMenu"));
            _no.Focus();
        }
        else if (_mainMenu.rootVisualElement.style.visibility == Visibility.Visible){ 
            EventSystem.SetUITookitEventSystemOverride(null, true, false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("MainMenu"));
            _start.Focus();
        }
        else if (_endscreen.rootVisualElement.style.visibility == Visibility.Visible){ 
            EventSystem.SetUITookitEventSystemOverride(null, true, false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("EndScreen"));
            _restart.Focus();
        }
        else if (_pauseMenu.rootVisualElement.style.visibility == Visibility.Visible){ 
            EventSystem.SetUITookitEventSystemOverride(null, true, false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find("PauseMenu"));
            _continue.Focus();
        } else EventSystem.SetUITookitEventSystemOverride(null, false, false);
    }

    private void OnApplicationQuit()
    {
        
        JSONConverter.jsonConverter.writeData((float)Convert.ToInt32(_mute.value), "mute");
        JSONConverter.jsonConverter.writeData(_sound.value, "sound");
        JSONConverter.jsonConverter.writeData(_music.value, "music");
        JSONConverter.jsonConverter.writeData((float)Convert.ToInt32(_fullscreen.value), "fullscreen");
        JSONConverter.jsonConverter.writeData((float)Convert.ToInt32(_saveIndex), "resolution");
    }
    #endregion Initialization

    #region Functionality

    private void Update()
    {
        // runtime UI Values
            _heartLabel.label = "x" + _playerControl._lives;
            if(_playerControl._keys == 0)
            {
                _key.style.visibility = Visibility.Hidden;
                _keyLabel.style.visibility = Visibility.Hidden;
            }
            else 
            {
                _key.style.visibility = Visibility.Visible;
                _keyLabel.style.visibility = Visibility.Visible;
                _keyLabel.label = "x" + _playerControl._keys;

            }
        _isPaused = _playerControl._isPaused;
        if 
        (
            _settingsMenu.rootVisualElement.style.visibility == Visibility.Visible || 
            _mainMenu.rootVisualElement.style.visibility == Visibility.Visible ||
            _endscreen.rootVisualElement.style.visibility == Visibility.Visible ||
            _confirmationMenu.rootVisualElement.style.visibility == Visibility.Visible
        )
        {
            _isPaused = true;
            _playerControl._isPaused = _isPaused;
        }
        Time.timeScale = _isPaused == true ? 0f : 1f;
        _pauseMenu.rootVisualElement.style.visibility = _isPaused == true ? Visibility.Visible : Visibility.Hidden;

        if(_isPaused) AudioManager._instance._audioMixer.SetFloat("Sounds",0f);
        else if (_isPaused && !_mute.value) AudioManager._instance._audioMixer.SetFloat("Sounds",_sound.value);

        if (_playerControl._isDead) 
        {
            _endscreen.rootVisualElement.style.visibility = Visibility.Visible;
        }
        else _endscreen.rootVisualElement.style.visibility = Visibility.Hidden;

    }

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

    #region ConfirmationMenu

    [SerializeField] private UIDocument _confirmationMenu;

    private Button _yes;
    private Button _no;

    #endregion ConfirmationMenu

    #region SettingMenu
    [SerializeField] private UIDocument _settingsMenu;
    private VisualElement _settingsPanel;
    private Button _exitSM;
    private Toggle _mute;
    private Slider _sound;
    private Slider _music;
    private Toggle _fullscreen;
    private DropdownField _resolution;
    private Resolution[] _resolutions;
    private int _resIndex;
    private int _saveIndex;
    private int[] _widths;
    private int[] _heights;

    #endregion SettingsMenu
}
