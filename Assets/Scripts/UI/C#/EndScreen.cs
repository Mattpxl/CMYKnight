using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    public UIDocument _endscreen;
    private AudioSource _audioSource;
    private Button _restart;
    private Button _quit;

    public bool quit, restart = false;

    void Awake()
    {
        _endscreen = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _restart = _endscreen.rootVisualElement.Q("btnRestart") as Button;
        _quit = _endscreen.rootVisualElement.Q("btnQuit") as Button;
    }
    void Start()
    {
        isDisabled();
        initCallbacks();
    }
    void OnEnable()
    {
        initCallbacks();
    }

    public void initCallbacks()
    {
       _quit.RegisterCallback<ClickEvent>((evt) => 
        { 
            quit = true;
        });
        _quit.RegisterCallback<NavigationSubmitEvent>((evt) => 
        { 
            quit = true;
        });
        _quit.RegisterCallback<NavigationMoveEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
        });
        _quit.RegisterCallback<PointerEnterEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            _quit.Focus();
        });
        _restart.RegisterCallback<ClickEvent>((evt) => 
        { 
            restart = true;
        });
        _restart.RegisterCallback<NavigationSubmitEvent>((evt) => 
        { 
            restart = true;
        });
        _restart.RegisterCallback<NavigationMoveEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
        });
        _restart.RegisterCallback<PointerEnterEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            _restart.Focus();
        });
    }
    public void isEnabled()
    {
       //_endscreen.enabled = true;
       //this.gameObject.SetActive(true);
        _endscreen.rootVisualElement.style.visibility = Visibility.Visible;
        //initCallbacks();
        quit = false;
        restart = false;
    }
    public void isDisabled()
    {
        if(_endscreen.isActiveAndEnabled)
        {
        _endscreen.rootVisualElement.style.visibility = Visibility.Hidden;
       // _endscreen.enabled = false;
       // this.gameObject.SetActive(false);
        }
    }
}
