using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pushable : MonoBehaviour
{
    #region Initialization
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private ContactFilter2D _layerFilter;
    [SerializeField] private LevelManager _levelManager;

    [Header("Attributes")]
    [SerializeField] private bool _isPushable;
    [SerializeField] private float _pushForce;
    [SerializeField] private float _resistance;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    #endregion Initialization

    #region Updates
    private void FixedUpdate()
    {
        push();
    }

    #endregion Updates

    #region Collisions

    [Header("Collisions")]
    [SerializeField] private Transform _leftCheck;
    [SerializeField] private Transform _rightCheck;
    [SerializeField] private float radius;
    private Vector2 _velRef;
    private void push()
    {
        if (_isPushable)
        {
            if (Physics2D.OverlapCircleAll(_leftCheck.position, radius, _levelManager.playerLayer).Length > 0)
            {
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_rigidbody.velocity.x * (_pushForce - _resistance), _rigidbody.velocity.y), 
                    ref _velRef,
                    0.1f
                );
            }
            if (Physics2D.OverlapCircleAll(_rightCheck.position, radius, _levelManager.playerLayer).Length > 0)
            {
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(-(_rigidbody.velocity.x * (_pushForce - _resistance)), _rigidbody.velocity.y), 
                    ref _velRef,
                    0.1f
                );
            }
            
        } 

    }

    #endregion Collisons
}