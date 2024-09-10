using UnityEngine;
using UnityEngine.SceneManagement;
using DataStorage;
using UnityEngine.UIElements;


public class Splash : MonoBehaviour
{
    public UIDocument _splash;
    private static PlayerControl _playerControl;
    private TableEntry _playerLevel;
    private float _level;

    private void Awake()
    {
        _playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
        _playerLevel = new TableEntry
        (
            "Level",
            "INT",
            1
        );
    }
    private void Start()
    {
       _level = _playerControl.loadPlayerValue(_playerLevel)._floatValue;
    }
    public void startGame(){
        _splash.rootVisualElement.style.visibility = Visibility.Hidden;
        SceneManager.LoadScene((int)_level == 0? 1 : (int)_level); 
    }

}
