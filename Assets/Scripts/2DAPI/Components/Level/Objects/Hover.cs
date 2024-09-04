using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float _speed;
    [SerializeField] private float _dirTimer;
    [SerializeField] private bool _isHorizontal;
    [SerializeField] private bool _isVertical;
    private bool _canChangeDir = true;
    private Vector2 _velRef;
  
    // Update is called once per frame
    void FixedUpdate()
    {
        if(_canChangeDir) StartCoroutine(setDirection());
    }
    // PUT CLAMP MAGNITUDE ON THESE VECTORS
    private IEnumerator setDirection()
    {
        _canChangeDir = false;
        if(_isHorizontal == true) transform.position = Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x + _speed,transform.position.y),
            ref _velRef,
            0.1f
        );
        if(_isVertical == true) transform.position = Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x, transform.position.y + _speed),
            ref _velRef,
            0.1f
        );
        yield return new WaitForSeconds(_dirTimer *  Time.fixedDeltaTime);
        if(_isHorizontal == true) transform.position = Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x + _speed,transform.position.y),
            ref _velRef,
            0.1f
        );
        if(_isVertical == true) transform.position = Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x, transform.position.y + _speed),
            ref _velRef,
            0.1f
        );
        yield return new WaitForSeconds(_dirTimer *  Time.fixedDeltaTime);
        _speed *= -1;
        _canChangeDir = true;
    }
}
