using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationMenu : MonoBehaviour
{

    public UIDocument _confirmationMenu;
    private Button _yes;
    private Button _no;

    public bool yes, no = false;

    void Awake()
    {
        _confirmationMenu = GetComponent<UIDocument>();
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
        _no.RegisterCallback<ClickEvent>((evt) => 
        { 
            no = true;
        });
    }
    public void isEnabled()
    {
       // _confirmationMenu.enabled = true;
        //this.gameObject.SetActive(true);
        _confirmationMenu.rootVisualElement.style.visibility = Visibility.Visible;
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
