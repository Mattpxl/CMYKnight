using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : MonoBehaviour
{
    public Transform _spawnPoint;

    private void Awake()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        }
        else _spawnPoint = transform;
    }
    void Start()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        }
        else _spawnPoint = transform;
    }

    void Update()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0) && _spawnPoint == transform)
        {
            _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        }
    }
}
