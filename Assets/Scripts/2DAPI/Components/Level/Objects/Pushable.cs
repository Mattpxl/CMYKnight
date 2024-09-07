using UnityEngine;


public class Pushable : MonoBehaviour
{
    #region Initialization
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private ContactFilter2D _layerFilter;
    [SerializeField] private LevelManager _levelManager;
    private AudioSource _audioSource;

    [Header("Attributes")]
    [SerializeField] private bool _isPushable;
    [SerializeField] private float _pushForce;
    [SerializeField] private float _resistance;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _audioSource = GetComponent<AudioSource>();
    }
 

    #endregion Initialization

    #region Updates
    private void FixedUpdate()
    {
        topCheck();
        push();
    }

    #endregion Updates

    #region Collisions

    [Header("Collisions")]
    [SerializeField] private Transform _leftCheck;
    [SerializeField] private Transform _rightCheck;
    [SerializeField] private Transform _topCheck;
    [SerializeField] private float radius;
    private bool isGrounded, onTop;

    public bool _canMove = true;
    private Vector2 _velRef;

    private void topCheck()
    {
        if(Physics2D.OverlapCircleAll(_topCheck.position, radius/4, _levelManager.playerLayer).Length > 0)
        {
            onTop = true;
        }
        else onTop = false;
    }
    private void push()
    {   
        if(_isPushable && !onTop ){
            if 
            (
                Physics2D.OverlapCircleAll(_leftCheck.position, radius * 4, _levelManager.playerLayer).Length > 0
                &&
                Physics2D.OverlapCircleAll(_rightCheck.position, radius/4, _levelManager.groundLayer).Length <= 0
            )
            {
                _canMove = true;
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_rigidbody.velocity.x * (_pushForce - _resistance), _rigidbody.velocity.y), 
                    ref _velRef,
                    0.1f
                );
            } 
            else if 
            (
                Physics2D.OverlapCircleAll(_rightCheck.position, radius * 4, _levelManager.playerLayer).Length > 0
                &&
                Physics2D.OverlapCircleAll(_leftCheck.position, radius/4, _levelManager.groundLayer).Length <= 0
            )
            {
                _canMove = true;
                _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(-(_rigidbody.velocity.x * (_pushForce - _resistance)), _rigidbody.velocity.y), 
                    ref _velRef,
                    0.1f
                );
            } else _canMove = false;
        } 
    }

    #endregion Collisons
}