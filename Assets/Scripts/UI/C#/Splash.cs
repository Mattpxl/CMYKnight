using UnityEngine;
using UnityEngine.SceneManagement;
using DataStorage;


public class Splash : MonoBehaviour
{
    private static PlayerData _playerData = new PlayerData();
    private TableEntry _playerLevel;
    private float _level;

    private void Awake()
    {
        _playerLevel = new TableEntry
        (
            "Level",
            "INT",
            1
        );
        _playerData.initializeTable();
    }
    private void Start()
    {
       _level = _playerData.loadPlayerValue(_playerLevel)._floatValue;
    }
    public void OnEnter(){
        SceneManager.LoadScene((int)_level == 0? 1 : (int)_level); 
    }

}
