using UnityEngine;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    public UIDocument _endscreen;
    private Button _restart;
    private Button _quit;

    public bool quit, restart = false;

    void Awake()
    {
        _endscreen = GetComponent<UIDocument>();
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
        _quit.RegisterCallback<PointerLeaveEvent>((evt) => 
        { 
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
        _restart.RegisterCallback<PointerLeaveEvent>((evt) => 
        { 
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
