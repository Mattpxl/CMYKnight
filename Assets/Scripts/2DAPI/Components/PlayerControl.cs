using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DataStorage;
using UnityEngine.SceneManagement;

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
/// </summary>
public class PlayerControl : MonoBehaviour
{
    /// <summary>
    /// Initlialize Components 
    /// </summary>
    #region Initialization
            private LayerManager _levelManager;
            [SerializeField] private SpawnPoint _spawnPoint;
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
        _levelManager = GetComponent<LayerManager>();
        _sqlConverter = new SQLConverter();
    }

    #endregion Initialization

    /// <summary>
    /// Save and Load Data
    /// > Create a TableEntry and [Default Value] TableEntry for new data
    /// > Add Table Entry to _playerDataSet & [Default Value] TableEntry to _playerAccessDataSet inside of initializeTable()
    /// > Add _tableEntry = loadPlayerValue(_playerTableEntry)._intValue;._floatValue;._stringValue; to Start()
    /// </summary>
   #region Data
    private SQLConverter _sqlConverter;
    private TableEntry[] _playerDataSet;
    private static TableEntry _playerLevel;
    private static TableEntry _playerLives;
    private static TableEntry _playerKeys;
    private static string[] _valueTypes = new string[3]
            {
                "INT",
                "REAL", 
                "REAL",
            };
    private static string[] _valueNames = new string[3]
            {
                "Level",
                "Lives", 
                "Keys",
            };
    private static ValueTable  _playerData = new ValueTable
        (
            "PlayerData",
            _valueNames,
            _valueTypes
        );
    //primary access key
    private static TableEntry _playerTableID = new TableEntry
        (
            "PlayerData_id",
            "INT",
            1
        );
    private static TableEntry _playerLevelDefault = new TableEntry
        (
            "Level",
            "INT",
            1
        );
    private static TableEntry _playerLivesDefault = new TableEntry
        (
            "Lives",
            "REAL",
            3
        );
    private static TableEntry _playerKeysDefault = new TableEntry
        (
            "Keys",
            "REAL",
            0
        );
        private TableEntry[] _playerAccessDataSet = new TableEntry[4]
        {
            _playerTableID,
            _playerLevelDefault,
            _playerLivesDefault,
            _playerKeysDefault
        };
    
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
        _sqlConverter.generateTable(_playerData);
    }
     public TableEntry loadPlayerValue(TableEntry entry)
    {
        TableEntry defaultValue = new TableEntry();
        foreach (TableEntry e in _playerAccessDataSet)
        {
            if (e._valueName == entry._valueName) defaultValue = e;
        }
        TableEntry value = new TableEntry();
        switch (entry._valueType)
        {
            case "TEXT":
                value = _sqlConverter.getValue(_playerData, entry, _playerTableID)._stringValue == null ? defaultValue : _sqlConverter.getValue(_playerData, entry, _playerTableID);
                break;
            case "REAL":
                value = _sqlConverter.getValue(_playerData, entry, _playerTableID)._floatValue == 0 ? defaultValue : _sqlConverter.getValue(_playerData, entry, _playerTableID);
                break;
            case "INT":
                value = _sqlConverter.getValue(_playerData, entry, _playerTableID)._intValue == 0 ? defaultValue : _sqlConverter.getValue(_playerData, entry, _playerTableID);
                break;
        }
        return value;
    }

    private void Start()
    {
        initializeTable();
        try
        {
            _level = loadPlayerValue(_playerLevel)._intValue;
        }
        catch
        {
            _level = _playerLevelDefault._intValue;
        }
        try
        {
            _lives = loadPlayerValue(_playerLives)._floatValue;
        }
        catch
        {
            _lives = _playerLivesDefault._floatValue;
        }
        try
        {
            _keys = loadPlayerValue(_playerKeys)._floatValue;
        }
        catch
        {
            _keys = _playerLivesDefault._floatValue;
        }
        _audioSource.Stop();
    }
    private void OnApplicationQuit()
    {
        try
        {
            _sqlConverter.updateTable(_playerData, _playerDataSet, _playerTableID);
        }
        catch
        {
            Debug.Log("PLayerControl: -Data Failed To Save-\n");
        }
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
            _lives--;
            isGrounded = _rigidbody.velocity.y <= 0;
        }
        else  
        {
            _isDead = true; 
        } 
        
    }

    public void dead()
    {
        if(_isDead == true)
        {
            isGrounded = false;
            AudioManager._instance._musicSource.Stop();
            _audioSource.Stop();
            if(_audioSource.isPlaying == false)_audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[4]._sound);
        }
    }
    public void spawn() 
    { 
        _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[3]._sound);
        transform.position = _spawnPoint._spawnPoint.position;
        _isDead = false; 
        if (_lives <= 0f) _lives = 3f;
    }
    public void respawn() 
    { 
        _audioSource.Stop();
        _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[3]._sound);
        transform.position = _spawnPoint._spawnPoint.position; 
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
        wallCheck();
        collectCheck();
        interactableCheck();
        hazardCheck();
        // Movement
        move();
        stopMovingX();
        jump();
        nudge();
        assistUpward();
        modifyApex();
        wallSlide();
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
    public bool _startGame;
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
    }
    private void OnPause()
    {
        _isPaused = !_isPaused;
        _audioSource.Stop();
        _audioSource.PlayOneShot(AudioManager._instance._sfxUI[0]._sound);
    }

    private void OnEnter()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
            _startGame = true;
    }

    #endregion Input

    /// <summary>
    /// Handles Walking / Running / pushing
    /// </summary>
    #region Horizontal Movement

    [Header("Horizontal Movement")]
    [SerializeField] private float _speed;
    private Vector2 _movementInput, _velocityRef;
    private bool _isMoving = false;
    private void move()
    {
        _isMoving = true;
        _rigidbody.velocity = Vector2.SmoothDamp
        (
            _rigidbody.velocity,
            new Vector2(_movementInput.x * _speed, _rigidbody.velocity.y),
            ref _velocityRef,
            0.1f
        );
    }

    private void stopMovingX()
    {
        if(_isMoving = true && _rigidbody.velocity.x == 0)
        {
            _rigidbody.velocity = Vector2.SmoothDamp
            (
                _rigidbody.velocity,
                new Vector2(0f, -_rigidbody.velocity.y),
                ref _velocityRef,
                0.1f
            );
            _isFalling = true;
        }
        
    }

    #endregion Horizontal Movement

    /// <summary>
    /// Handles Jumping / falling / Crouching
    /// </summary>
    #region Vertical Movement

    [Header("Vertical Movement")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpMagnitude;
    [SerializeField] private float _fallSpeed;
    [SerializeField] private float _fallMagnitude;
    [SerializeField] private float _nudgeSpeed;
    [SerializeField] private float _nudgeMagnitude;
    [SerializeField] private float _upAssistSpeed;
    [SerializeField] private float _upAssistMagnitude;
    private bool _isFalling = false;
    private bool _isJumping = false;
    private bool _canAssist = true;
    private void jump()
    {
        if (_isJumping) {
        _canModify = true;
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
        else _isFalling = false;
        if(_isFalling == true)
        {
           _rigidbody.velocity = Vector2.ClampMagnitude 
            (
                Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * _fallSpeed),
                    ref _velocityRef,
                    0.1f
                ), 
                _fallMagnitude
            );
        }
    }
    private void land()
    {
        if (isGrounded && _isFalling) 
        {
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[2]._sound);
            _isFalling = false;
            _canAssist = true;
        }
    }
    private void modifyApex()
    {
        if(_rigidbody.velocity.y == 0)
        {
            StartCoroutine(ApexModifierDelay());
           if(_canModify)
           {
              _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
              _rigidbody.gravityScale = 0f;
           } 
           _rigidbody.gravityScale = 1f;
        }  
    }

    private void nudge()
    {
        if((hitCeiling == true && isGrounded == false))
        {
            _rigidbody.velocity = Vector2.ClampMagnitude 
            (
                 Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_rigidbody.velocity.x * _nudgeSpeed, _rigidbody.velocity.y * _nudgeSpeed),
                    ref _velocityRef,
                    0.1f
                ),
                _nudgeMagnitude
            );
        }
    }

    private void assistUpward()
    {
        if((hitRight || hitLeft) && _rigidbody.velocity.y > 0 && _canAssist == true) 
        {
             _rigidbody.velocity = Vector2.ClampMagnitude 
            (
                 Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_rigidbody.velocity.x * _upAssistSpeed, _rigidbody.velocity.y * _upAssistSpeed),
                    ref _velocityRef,
                    0.1f
                ),
                _upAssistMagnitude
            );
            _canAssist = false;
        }
    } 
    private void wallSlide()
    {
        if((hitRight || hitLeft) && _canAssist == false)
        {
            _rigidbody.velocity = Vector2.ClampMagnitude 
            (
                Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(0f, _rigidbody.velocity.y * _fallSpeed),
                    ref _velocityRef,
                    0.1f
                ), 
                _fallMagnitude
            );
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
    [SerializeField] private Transform leftCheckObj;
    [SerializeField] private Transform rightCheckObj;
    private const float radius = 0.1f;
    private bool isGrounded, wasGrounded, hitCeiling, hitLeft, hitRight;

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
        hitCeiling = Physics2D.OverlapCircleAll(ceilingCheckObj.position, 0.3f, _levelManager.groundLayer).Length > 0 ? true : false;
        platformCheck();
    }
    private void hazardCheck()
    {
      if(Physics2D.IsTouchingLayers(_collider, _levelManager.hazardLayer)  && _isImmune == false)
      {
        takeDamage();
        StartCoroutine(DamageImmunity());
      }
    }
    private void wallCheck()
    {
        hitLeft = (Physics2D.OverlapCircleAll(leftCheckObj.position, 0.5f, _levelManager.groundLayer).Length > 0 && isGrounded == false && transform.localScale.x == -1) ? true : false;
        hitRight = (Physics2D.OverlapCircleAll(rightCheckObj.position, 0.5f, _levelManager.groundLayer).Length > 0 && isGrounded == false && transform.localScale.x == 1) ? true : false;
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
                    && colliders[0].gameObject.GetComponent<Pushable>().onTop == false
                )
                {
                    colliders[0].gameObject.GetComponent<AudioSource>().PlayOneShot(AudioManager._instance._sfxWorld[1]._sound);
                }
            }
        }
    }

    private void platformCheck()
    {  
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ceilingCheckObj.position, 0.3f, _levelManager.platformLayer);
        if (colliders.Length > 0)
        {
            Physics2D.IgnoreLayerCollision(3, 14, true);
            StartCoroutine(PlatfromCollisonDelay());
                 
        }
        colliders = Physics2D.OverlapCircleAll(groundCheckObj.position, 0.3f, _levelManager.platformLayer);
        if(isGrounded == true && _canCollidePlatform == true && colliders.Length > 0)
        {
            Physics2D.IgnoreLayerCollision(3, 14, false);
            if(colliders[0].gameObject.GetComponent<Moveable>()._isHorizontal == true)
                _rigidbody.velocity = new Vector2 (colliders[0].gameObject.GetComponent<Rigidbody2D>().velocity.x * 1.15f, _rigidbody.velocity.y);
               
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
    [SerializeField] private float _apexModifier;
    private bool _canModify = true;

    private IEnumerator ApexModifierDelay()
    {
        _canModify = true;
        yield return new WaitForSeconds(_apexModifier);
        _canModify = false;
    }

    [SerializeField] private float _platformCollisionDelay;
    private bool _canCollidePlatform;

    private IEnumerator PlatfromCollisonDelay()
    {
        _canCollidePlatform = false;
        yield return new WaitForSeconds(_platformCollisionDelay);
        _canCollidePlatform = true;
    }

    #endregion Coroutines

}

