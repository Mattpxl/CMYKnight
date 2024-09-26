using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Splash _splashScreen;
    
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
        AudioManager._instance._sfxSource.loop = true;
    }
    #endregion Initialization

    private void Update()
    {
        //HandleSplashScreen();
        //HandleSettingsMenu();
        //HandleConfirmationMenu();
        //HandleMainMenu();
        //HandleEndScreen();
        //HandlePauseMenu();
        //UpdateRuntimeUI();
         if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            _mainMenu.SetMenuVisibility(false);

            if (_playerControl._startGame)
            {
                AudioManager._instance.playMusic("menu0");
                _splashScreen.StartGame();
                //_splashScreen.SetMenuVisibility(false);
                //_playerControl._startGame = false;
                _mainMenu.SetMenuVisibility(true);
            }
            else
            {
                _settingsMenu.SetMenuVisibility(false);
                _mainMenu.SetMenuVisibility(false);
                _confirmationMenu.SetMenuVisibility(false);
                _endscreen.SetMenuVisibility(false);
                _pauseMenu.SetMenuVisibility(false);
            }
        }
        else if (_settingsMenu.IsVisible())
        {
            if (_settingsMenu.isQuit)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[3]._sound);
                _mainMenu.isOpenSettings = false;
                _pauseMenu.isOpenSettings = false;
                _settingsMenu.SetMenuVisibility(false);
            }
        }
        else if (_confirmationMenu.IsVisible())
        {
            if (_confirmationMenu.isConfirmedYes)
            {
                AudioManager._instance._musicSource.Stop();
                AudioManager._instance._sfxSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                AudioManager._instance.playMusic("menu0");
                _mainMenu.SetMenuVisibility(true);
                _settingsMenu.SaveSettingsValues();
                _playerControl.SavePlayerData();
                _playerControl._resetLevel = true;
                _isPaused = false;
                _playerControl._isPaused = _isPaused;
                _confirmationMenu.SetMenuVisibility(false);
                _pauseMenu.SetMenuVisibility(false);
                _endscreen.SetMenuVisibility(false);
                _splashScreen.SetMenuVisibility(true);
            }

            if (_confirmationMenu.isConfirmedNo)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                _pauseMenu.isMainMenu = false;
                _endscreen.isQuit = false;
                _confirmationMenu.SetMenuVisibility(false);
            }
        } 
        else if (_mainMenu.IsVisible())
        {
            _splashScreen.SetMenuVisibility(false);
            Time.timeScale = 0f;
            AudioManager._instance.SetAudioMixerLevels("Player", "World", "Enemy", -80f);

            if (_mainMenu.isCanStart)
            {
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[4]._sound);
                AudioManager._instance.playSfx("dungeon");
                AudioManager._instance.playMusic("cavern");

                _isPaused = false;
                _playerControl._isPaused = _isPaused;
                if(_playerControl._isDead) _playerControl.respawn(true);
                else _playerControl.respawn();
                _mainMenu.SetMenuVisibility(false);
                _confirmationMenu.SetMenuVisibility(false);
            }

            else if (_mainMenu.isOpenSettings)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _settingsMenu.SetMenuVisibility(true);
            }

            else if (_mainMenu.isQuit)
            {
                AudioManager._instance._musicSource.Stop();
                AudioManager._instance._sfxSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[5]._sound);
                //AudioManager._instance.playMusic("shrine");
                SceneManager.LoadScene(0);
                _playerControl._startGame = false;
                _splashScreen.SetMenuVisibility(true);
            }
        }
        else if (_endscreen.IsVisible())
        {
            Time.timeScale = 0f;

            if (_endscreen.isQuit)
            {
                _audioSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _confirmationMenu.SetMenuVisibility(true);
            }

            else if (_endscreen.isRestart)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                AudioManager._instance._musicSource.Stop();
                AudioManager._instance._sfxSource.Stop();
                AudioManager._instance.playMusic("cavern");
                AudioManager._instance.playSfx("dungeon");

                _endscreen.SetMenuVisibility(false);
                _isPaused = false;
                _playerControl._isPaused = _isPaused;
                _playerControl.respawn(true);
            }
        }
        else if (_pauseMenu.IsVisible())
        {
            if (_pauseMenu.isContinue || !_playerControl._isPaused)
            {
                AudioManager._instance._musicSource.Stop();
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[1]._sound);
                AudioManager._instance.playSfx("dungeon");
                AudioManager._instance.playMusic("cavern");

                _isPaused = false;
                _playerControl._isPaused = _isPaused;
                _pauseMenu.SetMenuVisibility(false);
            }

            else if (_pauseMenu.isOpenSettings)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _settingsMenu.SetMenuVisibility(true);
            }

            else if (_pauseMenu.isMainMenu)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxUI[2]._sound);
                _confirmationMenu.SetMenuVisibility(true);
            }
        }
        else 
        {
        _runtimeUI.HeartLabel.label = "x" + _playerControl._lives;
        _runtimeUI.CoinLabel.label = "x" + _playerControl._coins;

        if (_playerControl._keys == 0)
        {
            _runtimeUI.Key.style.visibility = Visibility.Hidden;
            _runtimeUI.KeyLabel.style.visibility = Visibility.Hidden;
        }
        else
        {
            _runtimeUI.Key.style.visibility = Visibility.Visible;
            _runtimeUI.KeyLabel.style.visibility = Visibility.Visible;
            _runtimeUI.KeyLabel.label = "x" + _playerControl._keys;
        }
        if (_playerControl._cKeys == 0)
        {
            _runtimeUI.Key1.style.visibility = Visibility.Hidden;
            _runtimeUI.Key1Label.style.visibility = Visibility.Hidden;
        }
        else
        {
            _runtimeUI.Key1.style.visibility = Visibility.Visible;
            _runtimeUI.Key1Label.style.visibility = Visibility.Visible;
            _runtimeUI.Key1Label.label = "x" + _playerControl._cKeys;
        }
        if (_playerControl._mKeys == 0)
        {
            _runtimeUI.Key2.style.visibility = Visibility.Hidden;
            _runtimeUI.Key2Label.style.visibility = Visibility.Hidden;
        }
        else
        {
            _runtimeUI.Key2.style.visibility = Visibility.Visible;
            _runtimeUI.Key2Label.style.visibility = Visibility.Visible;
            _runtimeUI.Key2Label.label = "x" + _playerControl._mKeys;
        }
        if (_playerControl._sKeys == 0)
        {
            _runtimeUI.Key3.style.visibility = Visibility.Hidden;
            _runtimeUI.Key3Label.style.visibility = Visibility.Hidden;
        }
        else
        {
            _runtimeUI.Key3.style.visibility = Visibility.Visible;
            _runtimeUI.Key3Label.style.visibility = Visibility.Visible;
            _runtimeUI.Key3Label.label = "x" + _playerControl._sKeys;
        }
        

        if (_playerControl._isDead)
        {
            _endscreen.SetMenuVisibility(true);
        }
        else
        {
            _endscreen.SetMenuVisibility(false);
        }

        _isPaused = _playerControl._isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            AudioManager._instance._sfxSource.Stop();
            _pauseMenu.SetMenuVisibility(true);
            AudioManager._instance.SetAudioMixerLevels("Player", "World", "Enemy", -80f);
        }
        else
        {
            Time.timeScale = 1f;
            AudioManager._instance.playSfx("dungeon");
            _pauseMenu.SetMenuVisibility(false);
            AudioManager._instance.SetAudioMixerLevels("Player", "World", "Enemy", -20f);
        }
        }
    }


}
