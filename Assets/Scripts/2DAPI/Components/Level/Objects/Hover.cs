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
    [SerializeField] private float _hoverMangnitude;
    private bool _canChangeDir = true;
    private Vector2 _velRef;
    void FixedUpdate()
    {
        if(_canChangeDir) StartCoroutine(setDirection());
    }
    private IEnumerator setDirection()
    {
        _canChangeDir = false;
        if(_isHorizontal == true) transform.position = Vector2.ClampMagnitude (
        Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x + _speed,transform.position.y),
            ref _velRef,
            0.1f
        ),
        _hoverMangnitude
        );
        if(_isVertical == true) transform.position = Vector2.ClampMagnitude (
        Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x, transform.position.y + _speed),
            ref _velRef,
            0.1f
        ),
        _hoverMangnitude
        );
        yield return new WaitForSeconds(_dirTimer *  Time.fixedDeltaTime);
        if(_isHorizontal == true) transform.position = Vector2.ClampMagnitude (
        Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x + _speed,transform.position.y),
            ref _velRef,
            0.1f
        ),
        _hoverMangnitude
        );
        if(_isVertical == true) transform.position = Vector2.ClampMagnitude (
        Vector2.SmoothDamp
        (
            transform.position,
            new Vector2(transform.position.x, transform.position.y + _speed),
            ref _velRef,
            0.1f
        ),
        _hoverMangnitude
        );
        yield return new WaitForSeconds(_dirTimer *  Time.fixedDeltaTime);
        _speed *= -1;
        _canChangeDir = true;
    }
}
