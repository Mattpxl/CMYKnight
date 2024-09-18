using UnityEngine;
using UnityEngine.UIElements;
using System;
using DataStorage;

public class SettingsMenu : MonoBehaviour
{
    #region Initialization
    public UIDocument _settingsMenu;
    private AudioSource _audioSource;
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

    public bool isQuit = false;

    private void Awake()
    {
        _settingsMenu = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _exitSM = _settingsMenu.rootVisualElement.Q<Button>("btnExitSM");
        _mute = _settingsMenu.rootVisualElement.Q<Toggle>("tglMute");
        _sound = _settingsMenu.rootVisualElement.Q<Slider>("sdrSound");
        _music = _settingsMenu.rootVisualElement.Q<Slider>("sdrMusic");
        _fullscreen = _settingsMenu.rootVisualElement.Q<Toggle>("tglFullScreen");
        _resolution = _settingsMenu.rootVisualElement.Q<DropdownField>("drpResolution");

        if (_exitSM == null || _mute == null || _sound == null || _music == null || _fullscreen == null || _resolution == null)
        {
            Debug.LogError("SettingsMenu: One or more UI elements are missing.");
        }
    }
    #endregion Initialization

    #region Data
    void Start()
    {
        LoadSettings();
        InitializeResolutions();
        SetMenuVisibility(false);
        InitCallbacks();
    }

    private void LoadSettings()
    {
        // Load mute setting
        try
        {
           // _mute.value = JSONConverter.jsonConverter.getDataObject<bool>("mute");
            _mute.value = Convert.ToBoolean(PlayerPrefs.GetInt("mute"));
        }
        catch
        {
            _mute.value = false;
        }

        // Load sound volume
        try
        {
            _sound.value = PlayerPrefs.GetFloat("sound");
            // JSONConverter.jsonConverter.getDataObject<float>("sound");
        }
        catch
        {
            _sound.value = 100f;
        }
        SetAudioLevel("Sounds", _sound.value - 80f);

        // Load music volume
        try
        {
            _music.value = PlayerPrefs.GetFloat("music");
            //JSONConverter.jsonConverter.getDataObject<float>("music");
        }
        catch
        {
            _music.value = 100f;
        }
        SetAudioLevel("Music", _music.value - 80f);

        // Load fullscreen setting
        try
        {
            _fullscreen.value = Convert.ToBoolean(PlayerPrefs.GetInt("fullscreen"));
            //Convert.ToBoolean(JSONConverter.jsonConverter.getDataObject<int>("fullscreen"));
        }
        catch
        {
            _fullscreen.value = false;
        }
    }

    private void InitializeResolutions()
    {
        _resolutions = Screen.resolutions;
        _widths = new int[_resolutions.Length];
        _heights = new int[_resolutions.Length];

        for (int i = 0; i < _resolutions.Length; i++)
        {
            //if (_resolutions[i].width % 16 == 0 && _resolutions[i].height % 9 == 0)
            //{
                _resolution.choices.Add(_resolutions[i].width + " x " + _resolutions[i].height);
                _widths[i] = _resolutions[i].width;
                _heights[i] = _resolutions[i].height;
            //}

            if (_resolutions[i].width == Screen.currentResolution.width && _resolutions[i].height == Screen.currentResolution.height)
            {
                _resIndex = i;
            }
        }

        try
        {
            _resolution.value = _resolution.choices[PlayerPrefs.GetInt("resolution")];
            //_resolution.choices[JSONConverter.jsonConverter.getDataObject<int>("resolution")];
        }
        catch
        {
            _resolution.value = _resolution.choices[_resolution.index];
            _saveIndex = _resolution.index;
        }
    }

    private void SetAudioLevel(string groupName, float level)
    {
        AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups($"{groupName}");
        AudioManager._instance._audioMixerGroup[0].audioMixer.SetFloat(groupName, level);
    }

    private void OnApplicationQuit()
    {
        SaveSettingsValues();
    }
    #endregion Data

    private void Update()
    {
        if (_isMute)
        {
            SetAudioLevel("Master", -80f);
        }
        else
        {
            SetAudioLevel("Master", -10f);
        }
    }

    private void InitCallbacks()
    {
        RegisterButtonCallbacks(_exitSM, () => isQuit = true);
        RegisterToggleCallbacks(_mute, () => _isMute = !_isMute);
        RegisterSliderCallbacks(_sound, "Sounds");
        RegisterSliderCallbacks(_music, "Music");
        RegisterToggleCallbacks(_fullscreen, () => _fullscreen.value = !_fullscreen.value);
        _resolution.RegisterValueChangedCallback((evt) =>
        {
            Screen.SetResolution(_widths[_resolution.index], _heights[_resolution.index], Screen.fullScreen);
            _saveIndex = _resolution.index;
        });
    }

    private void RegisterButtonCallbacks(Button button, System.Action onClick)
    {
        button.RegisterCallback<ClickEvent>((evt) => onClick());
        button.RegisterCallback<NavigationSubmitEvent>((evt) => onClick());
        button.RegisterCallback<PointerEnterEvent>((evt) =>
        {
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            button.Focus();
        });
    }

    private void RegisterToggleCallbacks(Toggle toggle, System.Action onChange)
    {
        toggle.RegisterCallback<ClickEvent>((evt) => onChange());
        toggle.RegisterCallback<NavigationSubmitEvent>((evt) => onChange());
    }

    private void RegisterSliderCallbacks(Slider slider, string groupName)
    {
        slider.RegisterValueChangedCallback((evt) =>
        {
            SetAudioLevel(groupName, slider.value - 80f);
        });
    }

    public void SetMenuVisibility(bool isVisible)
    {
        _settingsMenu.rootVisualElement.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        if (isVisible)
        {
            _exitSM.Focus();
            isQuit = false;
        }
    }
    public bool IsVisible()
    {
        return _settingsMenu.rootVisualElement.style.visibility == Visibility.Visible;
    }
    public void SaveSettingsValues()
    {

        PlayerPrefs.SetInt("mute", Convert.ToInt32(_mute.value));
        PlayerPrefs.SetFloat("sound", _sound.value);
        PlayerPrefs.SetFloat("music", _music.value);
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(_fullscreen.value));
        PlayerPrefs.SetInt("resolution", _saveIndex);
        //JSONConverter.jsonConverter.writeData(_mute.value, "mute");
        //JSONConverter.jsonConverter.writeData(_sound.value, "sound");
        //JSONConverter.jsonConverter.writeData(_music.value, "music");
        //JSONConverter.jsonConverter.writeData(_fullscreen.value, "fullscreen");
        //JSONConverter.jsonConverter.writeData(_saveIndex, "resolution");
    }
}
