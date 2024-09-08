using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class StateMachine : MonoBehaviour
{
    #region Initialization
    [SerializeField] private PlayerControl _playerControl;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private SettingsMenu _settingsMenu;
    [SerializeField] private RunTimeUI _runtimeUI;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private ConfirmationMenu _confirmationMenu;
    [SerializeField] private EndScreen _endscreen;
    private bool _isPaused;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _isPaused = _playerControl._isPaused;
        AudioManager._instance._musicSource.loop = true;
        AudioManager._instance.playMusic("menu0");
        //_playerControl._playerInput.SwitchCurrentActionMap("UI");
    }

    #endregion Initialization

    private void Update()
    {
        // swap input from player to first focus element when paused and when menu is open, so forth for eavh get a starting point for nav to register
        // Setting Menu
        if(_settingsMenu._settingsMenu.rootVisualElement.style.visibility == Visibility.Visible) //_settingsMenu.isActiveAndEnabled == true
        {
            
            if(_settingsMenu.quit == true)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[3]._sound);
                _mainMenu._openSettings = false;
                _pauseMenu._openSettings = false;
                _settingsMenu.isDisabled();
            }
          }
          // Confirmation menu
          else if (_confirmationMenu._confirmationMenu.rootVisualElement.style.visibility == Visibility.Visible) //_confirmationMenu.isActiveAndEnabled == true
          {
            if(_confirmationMenu.yes == true)
            {
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                AudioManager._instance.playMusic("menu0");
                _mainMenu.isEnabled();
                _isPaused = false; 
                _playerControl._isPaused = _isPaused;
                _confirmationMenu.isDisabled();
                _pauseMenu.isDisabled();
                _endscreen.isDisabled();
            }
            if(_confirmationMenu.no == true)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                _confirmationMenu.isDisabled();
            }
          }
          //MainMenu
          else if (_mainMenu._mainMenu.rootVisualElement.style.visibility == Visibility.Visible)//_mainMenu.isActiveAndEnabled == true
          {
            //EventSystem.current.SetSelectedGameObject(_mainMenu._start);
            if (_mainMenu._canStart == true)
            {
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[4]._sound);
                AudioManager._instance.playMusic("cavern");
                
                _isPaused = false; 
                _playerControl._isPaused = _isPaused;
                
                _playerControl.spawn();
                _mainMenu.isDisabled();
                _confirmationMenu.isDisabled();
            }
            if(_mainMenu._openSettings == true)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _settingsMenu.isEnabled();
            }
            if(_mainMenu._quit == true)
            {
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                SceneManager.LoadScene(0);
            }
          }
          // End Screen
          else if (_endscreen._endscreen.rootVisualElement.style.visibility == Visibility.Visible) //_endscreen.isActiveAndEnabled == true
          {
            if(_endscreen.quit == true)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _confirmationMenu.isEnabled();
            }
            if(_endscreen.restart == true)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                AudioManager._instance._musicSource.Stop();
                AudioManager._instance.playMusic("cavern");
                _endscreen.isDisabled();
                
                _isPaused = false; 
                _playerControl._isPaused = _isPaused;
                
                _playerControl.respawn();
            }
          }
        // PauseMenu
        else if (_pauseMenu._pauseMenu.rootVisualElement.style.visibility == Visibility.Visible) //_pauseMenu.isActiveAndEnabled == true
        {
            if(_pauseMenu._cont == true || _playerControl._isPaused == false)
            {
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[1]._sound);
                AudioManager._instance.playMusic("cavern");
                
                _isPaused = false; 
                _playerControl._isPaused = _isPaused;
                _pauseMenu.isDisabled(); 
                
            }
            if(_pauseMenu._openSettings == true)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _settingsMenu.isEnabled();
            }
            if(_pauseMenu._mainMenu == true)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _confirmationMenu.isEnabled();
            }
        }
        // runtime UI Values
        else 
        {
            _runtimeUI._heartLabel.label = "x" + _playerControl._lives;
            if(_playerControl._keys == 0)
            {
            _runtimeUI._key.style.visibility = Visibility.Hidden;
            _runtimeUI._keyLabel.style.visibility = Visibility.Hidden;
            }
            else 
            {
            _runtimeUI._key.style.visibility = Visibility.Visible;
            _runtimeUI._keyLabel.style.visibility = Visibility.Visible;
            _runtimeUI._keyLabel.label = "x" + _playerControl._keys;
            }
            if (_playerControl._isDead) 
            {
                //_playerControl._playerInput.SwitchCurrentActionMap("UI");
                _endscreen.isEnabled();
            }
            else
            {
                //_playerControl._playerInput.SwitchCurrentActionMap("Player");
                _endscreen.isDisabled();
            }   

        }
         _isPaused = _playerControl._isPaused;
            if(_isPaused == true)
            {
                //_playerControl._playerInput.SwitchCurrentActionMap("UI");
                Time.timeScale = 0f;
                _pauseMenu.isEnabled();
                AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master/Sounds/");
                AudioManager._instance._audioMixerGroup[1].audioMixer.SetFloat("Player",  -80f);
                AudioManager._instance._audioMixerGroup[2].audioMixer.SetFloat("World",  -80f);
                AudioManager._instance._audioMixerGroup[3].audioMixer.SetFloat("Enemy",  -80f);
            }
            if(_isPaused == false)
            {
                //_playerControl._playerInput.SwitchCurrentActionMap("Player");
                Time.timeScale = 1f;
                _pauseMenu.isDisabled();         
                AudioManager._instance._audioMixerGroup = AudioManager._instance._audioMixer.FindMatchingGroups("Master/Sounds/");
                AudioManager._instance._audioMixerGroup[1].audioMixer.SetFloat("Player", 0f);
                AudioManager._instance._audioMixerGroup[2].audioMixer.SetFloat("World",  0f);
                AudioManager._instance._audioMixerGroup[3].audioMixer.SetFloat("Enemy",  -5f);
            }
    }

}
