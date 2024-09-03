using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/PauseMenu")]
    public static void ShowExample()
    {
        PauseMenu wnd = GetWindow<PauseMenu>();
        wnd.titleContent = new GUIContent("PauseMenu");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
