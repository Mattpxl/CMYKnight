using UnityEngine;


public class Pushable : MonoBehaviour
{
    #region Initialization
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private ContactFilter2D _layerFilter;
    private LayerManager _levelManager;
    private AudioSource _audioSource;

    [Header("Attributes")]
    [SerializeField] private float _pushForce;
    [SerializeField] private float _resistance;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
        _levelManager = GetComponent<LayerManager>();
    }
 

    #endregion Initialization

    #region Updates
    private void FixedUpdate()
    {
        topCheck();
        groundCheck();

        push();
        fallCheck();
        landCheck();
    }

    #endregion Updates

    #region Collisions

    [Header("Collisions")]
    [SerializeField] private Transform _leftCheck;
    [SerializeField] private Transform _rightCheck;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float radius;
    private bool isGrounded = true, onTop = false, isFalling = false, hasPlayed = false;

    private Vector2 _velRef;

    private void topCheck()
    {
        onTop = Physics2D.OverlapCircleAll(_topCheck.position, 0.28f, _levelManager.playerLayer).Length > 0 ? true : false;
    }
     private void groundCheck()
    {
        isGrounded = Physics2D.OverlapCircleAll(_groundCheck.position, 0.3f, _levelManager.groundLayer).Length > 0 ? true : false;
    }
    private void fallCheck()
    {
        if(isGrounded == false && _rigidbody.velocity.y < 0) isFalling = true;
    }
    private void landCheck()
    {
        if (isGrounded == true && isFalling == true) 
        {
            _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[4]._sound);
            isFalling = false;
        }
    }
    private void push()
    {   
            if(onTop == true)
            {
                _audioSource.Stop();
                _rigidbody.velocity = Vector2.zero;
            }
            if
            (
                (
                Physics2D.OverlapCircleAll(_rightCheck.position, 0.4f, _levelManager.playerLayer).Length > 0
                ||
                Physics2D.OverlapCircleAll(_leftCheck.position, 0.4f, _levelManager.playerLayer).Length > 0
                )
                &&
                (
                Physics2D.OverlapCircleAll(_rightCheck.position, 0.35f, _levelManager.groundLayer).Length > 0
                ||
                Physics2D.OverlapCircleAll(_leftCheck.position, 0.35f, _levelManager.groundLayer).Length > 0
                )
                &&
                _rigidbody.velocity.x != 0f 
                && hasPlayed == false
            )
            {
               
                _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[5]._sound);
                hasPlayed = true;
                _rigidbody.velocity = Vector2.zero;
                
                    
            }
            else if 
            (
                Physics2D.OverlapCircleAll(_leftCheck.position, radius , _levelManager.playerLayer).Length > 0
                &&
                Physics2D.OverlapCircleAll(_rightCheck.position, radius, _levelManager.groundLayer).Length <= 0
            )
            {
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2((_rigidbody.velocity.x * _pushForce)  - _resistance, _rigidbody.velocity.y), 
                    ref _velRef,
                    0.1f
                );
                hasPlayed = false;
            } 
            else if 
            (
                Physics2D.OverlapCircleAll(_rightCheck.position, radius, _levelManager.playerLayer).Length > 0
                &&
                Physics2D.OverlapCircleAll(_leftCheck.position, radius, _levelManager.groundLayer).Length <= 0
                
            )
            {
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(-(_rigidbody.velocity.x * (_pushForce)) + _resistance, _rigidbody.velocity.y), 
                    ref _velRef,
                    0.1f
                );
                hasPlayed = false;
            } 
            
        } 
    }

    #endregion Collisons
