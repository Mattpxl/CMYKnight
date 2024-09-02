using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Initialization
        [SerializeField] private LevelManager _levelManager;
        private Rigidbody2D _rigidbody;
        private CapsuleCollider2D _collider;
        private Animator _animator;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
    }

    #endregion Initialization

    #region Updates
    private Vector2 _velRef;
    // Update is called once per frame
    void FixedUpdate()
    {
        flipCheck();
        if(_canMove){
            StartCoroutine(moveTimer());
            _rigidbody.velocity = Vector2.SmoothDamp 
            (   
                _rigidbody.velocity,
                new Vector2(_speed * move(), 0f),
                ref _velRef,
                0.1f
            );
        }
    }

    #endregion Updates

    #region Behaviour

    [Header("Behaviour")]
    [SerializeField] private float _speed;

    private float move()
    {
            int dir = Random.Range(0,2) == 0 ? -1 : 1;
            transform.localScale = new Vector2(dir, 1);
            int move = Random.Range(0,2);
            _animator.SetFloat("Move", move > 0 ? 1 : 0);
            return dir * (move > 0 ? 1 : 0); 
    }

    #endregion Behaviour

    #region Collisions

    [Header("Collisions")]
    [SerializeField] private Transform wallCheck0;
    [SerializeField] private Transform wallCheck1;
    private float radius = 0.1f;

    private void flipCheck()
    {
        if(_canCheckWall == true) {
        if 
        (
            Physics2D.OverlapCircleAll(wallCheck0.position, radius, _levelManager.groundLayer).Length > 0 ||
            Physics2D.OverlapCircleAll(wallCheck1.position, radius, _levelManager.groundLayer).Length > 0
        )
        {
            transform.localScale *= new Vector2(-1, 1); 
            StartCoroutine(wallCheckTimer());
            StartCoroutine(moveTimer());
            _animator.SetFloat("Move", 1);
            _rigidbody.velocity = new Vector2(_speed / 2 * _rigidbody.velocity.x, 0f);
            _animator.SetFloat("Move", 0);
        }
        }

    }

    #endregion Collisions

    #region Coroutines
    private bool _canMove = true;
    private bool _canCheckWall = true;
    private IEnumerator moveTimer()
    {   
        _canMove = false;
        yield return new WaitForSeconds(1);
        _canMove = true;
    }
    private IEnumerator wallCheckTimer()
    {
        _canCheckWall = false;
        yield return new WaitForSeconds(2);
        _canCheckWall = true;
    }
    #endregion Coroutines
}
