using System.Collections;
using UnityEngine;

/// <summary>
/// Moveable Platforms Component
/// > Standard Moving Platforms (Horizontal / Vertical)
/// > Changes Direction when contact with ground occurs
/// * Preassure Plate Activated Platforms
/// * Platform falls/rises when player is on it (elevator)
/// </summary>
public class Moveable : MonoBehaviour
{
    #region Initialization
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private LayerManager _levelManager;

    [Header("Attributes")]
    [SerializeField] private bool _isMoving;
    [SerializeField] public bool _isHorizontal;
    [SerializeField] public bool _isVertical;
    [SerializeField] private float _speed;
    private AudioSource _audioSource;

    void Awake()
    {
        _isMoving = true;
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _levelManager = GetComponent<LayerManager>();
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(10,10,true);
        Physics2D.IgnoreLayerCollision(14,14,true);
        Physics2D.IgnoreLayerCollision(10,14,true);
    }

     #endregion Initialization


    #region Updates

    void FixedUpdate()
    {
            if ( _isMoving == false &&
            _isGrounded == true &&
            (_rigidbody.velocity.x != 0 || _rigidbody.velocity.y != 0)
            ) 
            if(this.gameObject.tag == "Platform") _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[2]._sound);
            _rigidbody.velocity = Vector2.zero;
            _isGrounded = false;
            
            if
            (
            _audioSource.isPlaying == false &&
            _isMoving == true 
            ) _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[3]._sound);

            move();
            groundCheck();
        
    }
    // check for player contact if pushable 

    #endregion Updates

    #region Behaviour

    private float _xDirection = 1f;
    private float _yDirection = 1f;
    private Vector2 _velRef;

    private void move()
    {
        if (_isMoving)
        {
            
            if (_isHorizontal)
            {
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_speed * _xDirection, _rigidbody.velocity.y),
                    ref _velRef,
                    0.1f
                );
            }
            
            if (_isVertical)
            {
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_rigidbody.velocity.x, _speed * _yDirection),
                    ref _velRef,
                    0.1f
                );
            }
        } 
    }

    #endregion Behaviour

    #region Collisons

   // [Header("Collisions")]
    private bool _flip = true;
    private bool _isGrounded = false;

    // check for stoppoints instead of ground to set them around the map and make a path 
    // Path intakes an array of transforms and moves the platform from obj to obj
    // [SerializeField] private Transform[] _path;
    private void groundCheck() 
    {   
        if (Physics2D.IsTouchingLayers(_collider, _levelManager.groundLayer))
        { 
            _isGrounded = true;
            if ( _isHorizontal && _flip)
            { 
                _xDirection *= -1;  
            }
            if ( _isVertical && _flip) 
            {
                _yDirection *= -1;
            }
            StartCoroutine(pauseTime());
            
        } 
    }

#endregion Collisions

#region Coroutines

[Header("Coroutines")]
[SerializeField] private float _pauseTime;
private IEnumerator pauseTime()
{
    _isMoving = false;
    StartCoroutine(flipDelay());
    yield return new WaitForSeconds(_pauseTime);
    _isMoving = true;
}

private IEnumerator flipDelay()
{
    _flip = false;
    yield return new WaitForSeconds(_pauseTime + 1f);
    _flip = true;
}

#endregion Coroutines

}
