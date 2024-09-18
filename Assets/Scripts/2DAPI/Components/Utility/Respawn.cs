using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Vector2 _origin;
    private PlayerControl _player;
    void Start()
    {
        _origin = transform.position;
        _player = GameObject.FindFirstObjectByType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_player._resetLevel == true)
        {
            transform.position = _origin;
        }
    }
}
