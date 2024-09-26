using UnityEngine;
using UnityEngine.UIElements;

public class RunTimeUI : MonoBehaviour
{
    #region Initialization

    public UIDocument RuntimeUI;
    public VisualElement collection;
    public VisualElement keyChain;
    public VisualElement Heart;
    public VisualElement Key;
    public VisualElement Key1;
    public VisualElement Key2;
    public VisualElement Key3;
    public VisualElement Coin;
    public TextField HeartLabel;
    public TextField CoinLabel;
    public TextField KeyLabel;
    public TextField Key1Label;
    public TextField Key2Label;
    public TextField Key3Label;

    public void Awake()
    {

        RuntimeUI = GetComponent<UIDocument>();
        keyChain = RuntimeUI.rootVisualElement.Q<VisualElement>("keyChain");
        collection = RuntimeUI.rootVisualElement.Q<VisualElement>("Collection");
        Coin = RuntimeUI.rootVisualElement.Q<VisualElement>("veCoin");
        Heart = RuntimeUI.rootVisualElement.Q<VisualElement>("veHeart");
        Key = RuntimeUI.rootVisualElement.Q<VisualElement>("veKey");
        Key1 = RuntimeUI.rootVisualElement.Q<VisualElement>("veKey1");
        Key2 = RuntimeUI.rootVisualElement.Q<VisualElement>("veKey2");
        Key3 = RuntimeUI.rootVisualElement.Q<VisualElement>("veKey3");
        HeartLabel = RuntimeUI.rootVisualElement.Q<TextField>("txtHeart");
        CoinLabel = RuntimeUI.rootVisualElement.Q<TextField>("txtCoin");
        KeyLabel = RuntimeUI.rootVisualElement.Q<TextField>("txtKey");
        Key1Label = RuntimeUI.rootVisualElement.Q<TextField>("txtKey1");
        Key2Label = RuntimeUI.rootVisualElement.Q<TextField>("txtKey2");
        Key3Label = RuntimeUI.rootVisualElement.Q<TextField>("txtKey3");

        if (Heart == null || Key == null || HeartLabel == null || KeyLabel == null)
        {
            Debug.LogError("RunTimeUI: One or more UI elements are missing.");
        }
    }
    #endregion Initialization
}
