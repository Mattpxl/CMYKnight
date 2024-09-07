using UnityEngine;
using UnityEngine.UIElements;

public class RunTimeUI : MonoBehaviour
{
    #region Initialization

    public UIDocument _runtimeUI;
    public VisualElement _heart;
    public VisualElement _key;
    public  TextField _heartLabel;
    public TextField _keyLabel;
    
    public void Awake()
    {
        _runtimeUI = GetComponent<UIDocument>();
        _heart = _runtimeUI.rootVisualElement.Q("veHeart");
        _key  = _runtimeUI.rootVisualElement.Q("veKey");
        _heartLabel = _runtimeUI.rootVisualElement.Q("txtHeart") as TextField;
        _keyLabel = _runtimeUI.rootVisualElement.Q("txtKey") as TextField;
    }
    #endregion Initialization
    void Start()
    {
        isEnabled();
    }

    public void isEnabled()
    {
       //_runtimeUI.enabled = true;
       //this.gameObject.SetActive(true);
        _runtimeUI.rootVisualElement.style.visibility = Visibility.Visible;
    }
    public void isDisabled()
    {
        
       // _runtimeUI.rootVisualElement.style.visibility = Visibility.Hidden;
        //_runtimeUI.enabled = false;
        this.gameObject.SetActive(false);
        
    }
}
