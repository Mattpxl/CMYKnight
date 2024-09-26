using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TravelPoint : MonoBehaviour
{
    private Collider2D _travelPoint;
    private LayerManager _levelManager;
    private PlayerControl _playerControl;

    private void Awake()
    {
        _travelPoint = GetComponent<Collider2D>();
        _levelManager = GetComponent<LayerManager>();
        _playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
    }
    private void FixedUpdate()
    {
        if(Physics2D.IsTouchingLayers(_travelPoint, _levelManager.playerLayer))
        {
            SceneManager.LoadScene(_playerControl._level);
            StartCoroutine(WaitForPlayer());
            _playerControl.respawn();
        }
    }

    private IEnumerator WaitForPlayer()
    {
        yield return new WaitForSeconds(3f);
    }
}
