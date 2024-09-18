using UnityEngine;
using UnityEngine.UIElements;

public class RunTimeUI : MonoBehaviour
{
    #region Initialization

    public UIDocument RuntimeUI;
    public VisualElement Heart;
    public VisualElement Key;
    public TextField HeartLabel;
    public TextField KeyLabel;

    public void Awake()
    {
        RuntimeUI = GetComponent<UIDocument>();
        Heart = RuntimeUI.rootVisualElement.Q<VisualElement>("veHeart");
        Key = RuntimeUI.rootVisualElement.Q<VisualElement>("veKey");
        HeartLabel = RuntimeUI.rootVisualElement.Q<TextField>("txtHeart");
        KeyLabel = RuntimeUI.rootVisualElement.Q<TextField>("txtKey");

        if (Heart == null || Key == null || HeartLabel == null || KeyLabel == null)
        {
            Debug.LogError("RunTimeUI: One or more UI elements are missing.");
        }
    }
    #endregion Initialization
}
