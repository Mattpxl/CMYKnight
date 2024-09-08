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
   
}
