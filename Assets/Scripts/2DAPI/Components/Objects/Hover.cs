using UnityEngine;

public class Hover : MonoBehaviour
{
    public float amplitude = 2;
    public float speed = 1.5f;
    private Vector2 _origin;
    private void Start()
    {
        _origin = transform.position;
    }
    private void Update() {
      transform.position = new Vector2(_origin.x, _origin.y + amplitude * Mathf.Cos(Time.time * speed));
    }
}
