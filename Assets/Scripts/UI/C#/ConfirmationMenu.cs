using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationMenu : MonoBehaviour
{

    public UIDocument _confirmationMenu;
    private AudioSource _audioSource;
    private Button _yes;
    private Button _no;

    public bool yes, no = false;

    void Awake()
    {
        _confirmationMenu = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _yes = _confirmationMenu.rootVisualElement.Q("btnYes") as Button;
        _no = _confirmationMenu.rootVisualElement.Q("btnNo") as Button;
    }
    void Start()
    {
        initCallbacks();
        isDisabled();
    }
    private void OnEnable()
    {
        initCallbacks();
    }
    private void initCallbacks()
    {
        _yes.RegisterCallback<ClickEvent>((evt) => 
        { 
            yes = true;
        });
         _yes.RegisterCallback<NavigationSubmitEvent>((evt) => 
        { 
            yes = true;
        });
        _yes.RegisterCallback<NavigationMoveEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
        });
        _yes.RegisterCallback<PointerEnterEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            _yes.Focus();
        });
        _no.RegisterCallback<ClickEvent>((evt) => 
        { 
            no = true;
        });
        _no.RegisterCallback<NavigationSubmitEvent>((evt) => 
        { 
            no = true;
        });
        _no.RegisterCallback<NavigationMoveEvent>((evt) => 
        { 
            _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
        });
        _no.RegisterCallback<PointerEnterEvent>((evt) => 
        {
             _audioSource.PlayOneShot(AudioManager._instance._sfxUI[6]._sound);
            _no.Focus();
        });
        
    }
    public void isEnabled()
    {
       // _confirmationMenu.enabled = true;
        //this.gameObject.SetActive(true);
        _confirmationMenu.rootVisualElement.style.visibility = Visibility.Visible;
        _no.Focus();
        //initCallbacks();
        yes = false;
        no = false;
    }
    public void isDisabled()
    {
       
        _confirmationMenu.rootVisualElement.style.visibility = Visibility.Hidden;
        //_confirmationMenu.enabled = false;
        //this.gameObject.SetActive(false);
        
    }
}
