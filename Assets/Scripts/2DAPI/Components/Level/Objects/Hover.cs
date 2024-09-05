using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] public float amplitude = 2;
    [SerializeField] public float speed = 1.5f;

    private UnityEngine.Vector2 _origin;

    private void Start()
    {
        _origin = transform.position;
    }

    private void Update() {
      transform.position = new UnityEngine.Vector2(_origin.x, _origin.y + amplitude * Mathf.Cos(Time.time * speed));
    }
}
