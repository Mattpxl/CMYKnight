using UnityEngine;
using UnityEngine.UIElements;
using System;
using DataStorage;
using System.Xml;

public class SettingsMenu : MonoBehaviour
{
    #region Initialization
    public UIDocument _settingsMenu;
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
    private bool _isMute = false;

    public bool quit = false;

    private void Awake()
    {
        _settingsMenu = GetComponent<UIDocument>();
        _exitSM  = _settingsMenu.rootVisualElement.Q("btnExitSM") as Button;
        _mute = _settingsMenu.rootVisualElement.Q("tglMute") as Toggle;
        _sound = _settingsMenu.rootVisualElement.Q("sdrSound") as Slider;
        _music = _settingsMenu.rootVisualElement.Q("sdrMusic") as Slider;
        _fullscreen = _settingsMenu.rootVisualElement.Q("tglFullScreen") as Toggle;
        _resolution = _settingsMenu.rootVisualElement.Q("drpResolution") as DropdownField;
    }
    #endregion Initialization

    #region Data
    void Start()
    {
        try
            {
                _mute.value = Convert.ToBoolean((int)JSONConverter.jsonConverter.getDataObject("mute"));
            }
            catch
            {
                _mute.value = false;
            }
            try 
            {
                _sound.value = JSONConverter.jsonConverter.getDataObject("sound");
            }
            catch 
            {
                _sound.value = 100f;
            }
            AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master/Sounds");
            AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat("Sounds", _sound.value - 80f);

            try 
            {
                _music.value = JSONConverter.jsonConverter.getDataObject("music");
            }
            catch 
            {
                _music.value = 100f;
            }
            AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master/Music");
            AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat("Music", _music.value - 80f);
            try
            {
                _fullscreen.value = Convert.ToBoolean((int)JSONConverter.jsonConverter.getDataObject("fullscreen"));
            }
            catch
            {
                _fullscreen.value = false;
            }
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
            try
            {
                _resolution.value = _resolution.choices[Convert.ToInt32(JSONConverter.jsonConverter.getDataObject("resolution"))];
            }
            catch
            {
                _resolution.value = _resolution.choices[_resolution.index];
                _saveIndex = _resolution.index;
            }
            isDisabled();
            initCallbacks();
    }

    private void OnApplicationQuit()
    {
        JSONConverter.jsonConverter.writeData((float)Convert.ToInt32(_mute.value), "mute");
        JSONConverter.jsonConverter.writeData(_sound.value, "sound");
        JSONConverter.jsonConverter.writeData(_music.value, "music");
        JSONConverter.jsonConverter.writeData((float)Convert.ToInt32(_fullscreen.value), "fullscreen");
        JSONConverter.jsonConverter.writeData((float)Convert.ToInt32(_saveIndex), "resolution");
    }
    #endregion Data

    private void Update()
    {
        if (_isMute)
        {
            AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master");
            AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat("Master",  -80f);
        }
        else 
        {
            AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master");
            AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat("Master", -10f);
        }
    }
    private void OnEnable()
    {
        initCallbacks();
    }
    private void initCallbacks()
    {
        _exitSM.RegisterCallback<ClickEvent>((evt) => 
        { 
            quit = true;
        });
        _exitSM.RegisterCallback<NavigationSubmitEvent>((evt) => 
        { 
            quit = true;
        });
        _exitSM.RegisterCallback<PointerEnterEvent>((evt) => 
        { 
            _exitSM.Focus();
        });
            _mute.RegisterCallback<ClickEvent>((evt) => 
            {
                _isMute = !_isMute;
            });
            _mute.RegisterCallback<NavigationSubmitEvent>((evt) => 
            {
                _isMute = !_isMute;
            });
            _sound.RegisterValueChangedCallback((evt) => 
            {
                AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master/Sounds");
                AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat("Sounds", _sound.value - 80f);
            });

            _music.RegisterValueChangedCallback((evt) => 
            {
                AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master/Music");
                AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat("Music", _music.value - 80f);
            });
            _fullscreen.RegisterCallback<ClickEvent>((evt) => 
            {
                _fullscreen.value = !_fullscreen.value;
            });
            _fullscreen.RegisterCallback<NavigationSubmitEvent>((evt) => 
            {
                _fullscreen.value = !_fullscreen.value;
            });
            _resolution.RegisterValueChangedCallback((evt) => 
            {
                Screen.SetResolution(_widths[_resolution.index],_heights[_resolution.index], Screen.fullScreen);
                _resolution.value = _resolution.choices[_resolution.index];
                _saveIndex = _resolution.index;
            });
    }
    public void isEnabled()
    {
       //_settingsMenu.enabled = true;
       //this.gameObject.SetActive(true);
        _settingsMenu.rootVisualElement.style.visibility = Visibility.Visible;
        _exitSM.Focus();
        initCallbacks();
        quit = false;
    }
    public void isDisabled()
    {
        _settingsMenu.rootVisualElement.style.visibility = Visibility.Hidden;
       // _settingsMenu.enabled = false;
        //this.gameObject.SetActive(false);
    }
}
