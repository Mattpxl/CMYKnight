using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : MonoBehaviour
{
    public Transform _spawnPoint;

    void Start()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        }
        else _spawnPoint = transform;
    }

    public void UpdateSpawn()
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            _spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        }
    }

}
