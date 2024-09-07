using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DataStorage;

/// <summary>
/// Player Control for 2D Platformer
/// > Initialization for components using Awake()
/// > Data - Start(), OnApplicationQuit()
/// > Updates - Update(), fixedUpdate()
/// > Input - handles messages from Input System, OnMove(), OnJump()
/// > Status - Health, Death, Respawn
/// > Horizontal Movement - walking / Running / Push
/// > Vertical Movement - Jumping / falling / Crouch
/// > Collisions - Ground / Ceiling Check, Hazard Check
/// > Coroutines - CoyoteTime, Damage Immunity 
/// * UI
/// </summary>
public class PlayerControl : MonoBehaviour
{
    /// <summary>
    /// Initlialize Components 
    /// </summary>
    #region Initialization
            [SerializeField] private LevelManager _levelManager;
            private Rigidbody2D _rigidbody;
            private CapsuleCollider2D _collider;
            private Animator _animator;
            private AudioSource _audioSource;

    private void Awake()
    {
        // Instantiate(prefab);
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    #endregion Initialization

    /// <summary>
    /// Save and Load Data
    /// > Create a TableEntry and [Default Value] TableEntry for new data
    /// > Add Table Entry to _playerDataSet & [Default Value] TableEntry to _playerAccessDataSet inside of initializeTable()
    /// > Add _tableEntry = loadPlayerValue(_playerTableEntry)._intValue;._floatValue;._stringValue; to Start()
    /// </summary>
   #region Data
    private static PlayerData _playerData = new PlayerData();
    private TableEntry[] _playerDataSet;
    private static TableEntry _playerLevel;
    private static TableEntry _playerLives;
    private static TableEntry _playerKeys;
    private void initializeTable()
    {
        _playerLevel = new TableEntry
        (
            "Level",
            "INT",
            0
        );
        _playerLives = new TableEntry
        (
            "Lives",
            "REAL",
            _lives
        );
        _playerKeys = new TableEntry
        (
            "Keys",
            "REAL",
            _keys
        );
        _playerDataSet = new TableEntry[3]
        {
            _playerLevel,
            _playerLives,
            _playerKeys
        };
    }
    private void Start()
    {
        initializeTable();
        _level = _playerData.loadPlayerValue(_playerLevel)._intValue;
        _lives = _playerData.loadPlayerValue(_playerLives)._floatValue;
        _keys = _playerData.loadPlayerValue(_playerKeys)._floatValue;
        _isPaused = true;
        _audioSource.Stop();
    }
    private void OnApplicationQuit()
    {
        _playerData.save(_playerDataSet);
    }

   #endregion Data

    /// <summary>
    /// Keep Track of Player Stats and Health
    /// </summary>
    #region Status

    [Header("Status")]
    public int _level = 0; // get from scene manager 
    [SerializeField] public float _lives;
    [SerializeField] public float _keys;
    public bool _isDead = false;

    private void takeDamage() 
    { 
        if(_lives >= 1) {
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[6]._sound);
            --_lives;
            isGrounded = _rigidbody.velocity.y <= 0;
        }
        else  
        {
            _isDead = true; 
        } 
        
    }

    public void dead()
    {
        if(_isDead)
        {
            isGrounded = false;
            AudioManager._instance._musicSource.Stop();
            _audioSource.Stop();
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[4]._sound);
        }
    }
    public void spawn() 
    { 
        _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[3]._sound);
        transform.position = _levelManager._spawnPoint.position;
        _isDead = false; 
        if (_lives <= 0f) _lives = 3f;
    }
    public void respawn() 
    { 
        _audioSource.Stop();
        _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[3]._sound);
        transform.position = _levelManager._spawnPoint.position; 
        _lives = 3f;
        _isDead = false; 
    }

    #endregion Status

   /// <summary>
   /// Handles Unity Update Functions
   /// > FixedUpdate()
   /// </summary>
   #region Updates

   // private void Update(){ } // runs on pause
    private void FixedUpdate()
    {
        // Collisions
        groundCheck();
        ceilingCheck();
        collectCheck();
        interactableCheck();
        hazardCheck();
        // Movement
        move();
        jump();
        fall();
        land();
        //status
        dead();
    }
    #endregion Updates
    
    /// <summary>
    /// Recieve Input from Unity Input System
    /// </summary>
    #region Input
    public bool _isPaused;
    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>() == Vector2.zero ? Vector2.zero : inputValue.Get<Vector2>();
        // animation
        _animator.SetFloat("xVelocity", inputValue.Get<Vector2>().x == -1f ? 1 : inputValue.Get<Vector2>().x);

        if(isGrounded)
        switch(UnityEngine.Random.Range(0,1)) 
        { 
            case 0: _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[0]._sound); break; 
            case 1: _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[1]._sound); break; 
            default: break;
        }
        // flip
        if (inputValue.Get<Vector2>().x != 0f && inputValue.Get<Vector2>().x < 0f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (inputValue.Get<Vector2>().x != 0f && inputValue.Get<Vector2>().x > 0f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
      private void OnJump()
    {
        if( (isGrounded || _coyoteJump) && _canJump == true)
        { 
            _isJumping = true;
            StartCoroutine(CanJumpDelay());
        }
        // apex modifiers - at the apex of the jump there is a moment of anti-gravity & maybe minor speed boost
        // if _isJumping == true and _rigidbody.velocity.y == 0f; moment of anti-g
        // clamped fall speed - allows landing on platforms while falling easier
        // variable jump height - add downward force when player is falling
        // edge detection for ceilings - move player slightly over and continue upward motion
        // catch edges when jumping on platfroms and nudge up
    }
    private void OnPause()
    {
        _isPaused = !_isPaused;
        _audioSource.Stop();
        _audioSource.PlayOneShot(AudioManager._instance._sfxUI[0]._sound);
    }

    #endregion Input

    /// <summary>
    /// Handles Walking / Running / pushing
    /// </summary>
    #region Horizontal Movement

    [Header("Horizontal Movement")]
    [SerializeField] private float _speed;
    private Vector2 _movementInput, _velocityRef;
    private void move()
    {
        _rigidbody.velocity = Vector2.SmoothDamp
        (
            _rigidbody.velocity,
            new Vector2(_movementInput.x * _speed, _rigidbody.velocity.y),
            ref _velocityRef,
            0.1f
        );
    }

    #endregion Horizontal Movement

    /// <summary>
    /// Handles Jumping / falling / Crouching
    /// </summary>
    #region Vertical Movement

    [Header("Vertical Movement")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpMagnitude;
    private bool _isFalling = false;
    private bool _isJumping = false;
    private void jump()
    {
        if (_isJumping) {
        _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[5]._sound);
        _rigidbody.velocity = Vector2.ClampMagnitude 
        (
            Vector2.SmoothDamp
            (
                _rigidbody.velocity,
                new Vector2(_rigidbody.velocity.x, _jumpForce * Vector2.up.y),
                ref _velocityRef,
                0.1f
            ), 
            _jumpMagnitude
        );
        _isJumping = false;
        }
    }
    private void fall()
    {
        if(isGrounded == false && _rigidbody.velocity.y < 0) _isFalling = true;
    }
    private void land()
    {
        if (isGrounded && _isFalling) 
        {
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[2]._sound);
            _isFalling = false;
        }
    }

    #endregion Vertical Movement

    /// <summary>
    /// Handles Collisions Against Layers
    /// > Ground (& ceiling), Hazard, 
    /// > Collectable - Heart, Key
    /// > Interactable - Door
    /// </summary>
    #region Collisions

    [Header("Collisions")]
    [SerializeField] private Transform groundCheckObj;
    [SerializeField] private Transform ceilingCheckObj;
    private const float radius = 0.1f;
    private bool isGrounded, wasGrounded, hitCeiling;

    private void groundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircleAll(groundCheckObj.position, radius, _levelManager.groundLayer).Length > 0 ? true : false;

        if (wasGrounded)
        { 
            StartCoroutine(CoyoteJumpDelay()); 
        }
        _animator.SetBool("isJumping", !isGrounded);
    }
    private void ceilingCheck()
    {
        hitCeiling = Physics2D.OverlapCircleAll(ceilingCheckObj.position, radius, _levelManager.groundLayer).Length > 0 ? true : false;
    }
    private void hazardCheck()
    {
      if(Physics2D.IsTouchingLayers(_collider, _levelManager.hazardLayer)  && _isImmune == false)
      {
        takeDamage();
        StartCoroutine(DamageImmunity());
      }
    }
    private void collectCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_rigidbody.position, 0.5f, _levelManager.collectableLayer);
        if (colliders.Length > 0)
        {
             if (colliders[0].gameObject.CompareTag("Heart"))
                {
                    _audioSource.PlayOneShot(AudioManager._instance._sfxItem[0]._sound);
                    Destroy(colliders[0].gameObject);
                    ++_lives;
                }
                else if (colliders[0].gameObject.CompareTag("Key"))
                {
                    _audioSource.PlayOneShot(AudioManager._instance._sfxItem[1]._sound);
                    Destroy(colliders[0].gameObject);
                    ++_keys;
                }
        }
    }
    private void interactableCheck()
    {
         Collider2D[] colliders = Physics2D.OverlapCircleAll(_rigidbody.position, 0.5f, _levelManager.interactableLayer);
        if (colliders.Length > 0)
        {
            if (colliders[0].gameObject.CompareTag("Door") && _keys >= 1)
            {
                _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[0]._sound);
                Destroy(colliders[0].gameObject);
                --_keys;
            } 
            else if(colliders[0].gameObject.CompareTag("Push Block"))
            {
                if
                (
                    colliders[0].gameObject.GetComponent<Rigidbody2D>().velocity.x != 0  && 
                    colliders[0].gameObject.GetComponent<AudioSource>().isPlaying == false
                )
                {
                    colliders[0].gameObject.GetComponent<AudioSource>().PlayOneShot(AudioManager._instance._sfxWorld[1]._sound);
                }
            }
        }
    }
   
    #endregion Collisions

    /// <summary>
    /// Place to put Coroutines
    /// > Coyote Jump
    /// > Damage Immunity
    /// </summary>
    #region Coroutines

    [Header("Coroutines")]
    [SerializeField] private float _coyoteTime;
    private bool _coyoteJump = false;
    private IEnumerator CoyoteJumpDelay()
    {
        _coyoteJump = true;
        yield return new WaitForSeconds(_coyoteTime);
        _coyoteJump = false;
    }
    [SerializeField] private float _jumpDelay;
    private bool _canJump = true;
    private IEnumerator CanJumpDelay()
    {
        _canJump = false;
        yield return new WaitForSeconds(_jumpDelay);
        _canJump = true;
    }

    [SerializeField] private float _immunityTime;
    private bool _isImmune = false;
    private IEnumerator DamageImmunity()
    {
        _isImmune = true;
        yield return new WaitForSeconds(_immunityTime);
        _isImmune = false;
    }
    #endregion Coroutines

}

