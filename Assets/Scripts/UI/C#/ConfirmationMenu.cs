using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmationMenu : MonoBehaviour
{
    public UIDocument _confirmationMenu;
    private AudioSource _audioSource;
    private Button _yes;
    private Button _no;

    public bool isConfirmedYes = false;
    public bool isConfirmedNo = false;

    void Awake()
    {
        _confirmationMenu = GetComponent<UIDocument>();
        _audioSource = GetComponent<AudioSource>();
        _yes = _confirmationMenu.rootVisualElement.Q("btnYes") as Button;
        _no = _confirmationMenu.rootVisualElement.Q("btnNo") as Button;

        if (_yes == null || _no == null)
        {
            Debug.LogError("ConfirmationMenu: One or both buttons are missing in the UI.");
        }
    }

    void Start()
    {
        InitCallbacks();
        SetMenuVisibility(false);
    }

   
    private void InitCallbacks()
    {
        RegisterButtonCallbacks(_yes, () => isConfirmedYes = true);
        RegisterButtonCallbacks(_no, () => isConfirmedNo = true);
    }

    private void RegisterButtonCallbacks(Button button, System.Action onSubmitAction)
    {
        var sound = AudioManager._instance._sfxUI[6]._sound;

        button.RegisterCallback<ClickEvent>((evt) => onSubmitAction());
        button.RegisterCallback<NavigationSubmitEvent>((evt) => onSubmitAction());
        button.RegisterCallback<NavigationMoveEvent>((evt) => _audioSource.PlayOneShot(sound));
        button.RegisterCallback<PointerEnterEvent>((evt) =>
        {
            _audioSource.PlayOneShot(sound);
            button.Focus();
        });
    }

    public void SetMenuVisibility(bool isVisible)
    {
        _confirmationMenu.rootVisualElement.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        if (isVisible)
        {
            isConfirmedYes = false;
            isConfirmedNo = false;
        }
    }
    public bool IsVisible()
    {
        return _confirmationMenu.rootVisualElement.style.visibility == Visibility.Visible;
    }
}
