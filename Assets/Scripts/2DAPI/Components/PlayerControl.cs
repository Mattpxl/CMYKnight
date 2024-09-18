using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DataStorage;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        
            public SpawnPoint _spawnPoint;
            private LayerManager _levelManager;
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
    }

    #endregion Initialization

    /// <summary>
    /// Save and Load Data
    /// > Create a TableEntry and [Default Value] TableEntry for new data
    /// > Add Table Entry to _playerDataSet & [Default Value] TableEntry to _playerAccessDataSet inside of initializeTable()
    /// > Add _tableEntry = loadPlayerValue(_playerTableEntry)._intValue;._floatValue;._stringValue; to Start()
    /// </summary>
   #region Data
  
    private List<TableEntry> _playerDataSet;
    private TableEntry _playerLevel;
    private TableEntry _playerLives;
    private TableEntry _playerKeys;
    private static readonly List<string> _valueTypes = new()
            {
                "INT",
                "REAL", 
                "REAL",
            };
    private static readonly List<string> _valueNames = new()
            {
                "Level",
                "Lives", 
                "Keys",
            };
    private static readonly ValueTable  _playerData = new ValueTable
        (
            "PlayerData",
            _valueNames,
            _valueTypes
        );
    //primary access key
    private static readonly TableEntry _playerTableID = new TableEntry
        (
            "PlayerData_id",
            "INT",
            1
        );
    // default values
    private static readonly Dictionary<string, TableEntry> _defaultValues = new()
    {
        {"Level", new TableEntry("Level", "INT", 1)},
        {"Lives", new TableEntry("Lives", "REAL", 3)},
        {"Keys", new TableEntry("Keys", "REAL", 0)}
    };

    private static readonly Dictionary<string, TableEntry> _cachedPlayerValues = new();
    
    private void initializeTable()
    {
        _playerLevel = new TableEntry("Level", "INT", _level);
        _playerLives = new TableEntry("Lives", "REAL", _lives);
        _playerKeys = new TableEntry ("Keys", "REAL", _keys);
        _playerDataSet = new() 
        {
            _playerLevel,
            _playerLives,
            _playerKeys
        };
        SQLConverter.sqlConverter.generateTable(_playerData);
        loadCachePlayerValues();
    }

    private void loadCachePlayerValues()
    {
        var playerValues = SQLConverter.sqlConverter.getValues(_playerData,new(){_playerTableID});

        foreach(var entry in playerValues)
        {   
            _cachedPlayerValues[entry._valueName] = entry;
        }
        foreach(var key in _valueNames)
        {
            if(!_cachedPlayerValues.ContainsKey(key))
            {
                _cachedPlayerValues[key] = _defaultValues[key];
            }
        }
    }
    public TableEntry loadPlayerValue(string valueName)
    {
        return _cachedPlayerValues.ContainsKey(valueName)
            ? _cachedPlayerValues[valueName]
            : _defaultValues[valueName];
    }
    private void Start()
    {
        initializeTable();
        
        _level = (int)loadPlayerValue("Level")._value;
        
        _lives = (float)(int)loadPlayerValue("Lives")._value;
        
        _keys = (float)(int)loadPlayerValue("Keys")._value;
        
        _audioSource.Stop();
    }
    private void OnApplicationQuit()
    {
        SavePlayerData();
    }

    public void SavePlayerData()
    {
         try
        {
            SQLConverter.sqlConverter.updateTable(_playerData, _playerDataSet, new(){_playerTableID});
        }
        catch
        {
            Debug.Log("PlayerControl: -Data Failed To Save-\n");
        }
    }

   #endregion Data

    /// <summary>
    /// Keep Track of Player Stats and Health
    /// </summary>
    #region Status
    public int _level { get; set; } 
    public float _lives { get; set; } 
    public float _keys { get; set; } 
    public bool _isDead { get; set; } = false;
    public bool _resetLevel { get; set; } = false;

    private void takeDamage() 
    { 
        if(_lives > 0) {
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[6]._sound);
            _lives--;
            isGrounded = _rigidbody.velocity.y <= 0;
        }
        else 
        {
            _isDead = true; 
            dead();
        } 
        
    }

    public void dead()
    {
        if(_isDead == true)
        {
            AudioManager._instance._musicSource.Stop();
            _audioSource.Stop();
            if(!_audioSource.isPlaying) _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[4]._sound);
            _resetLevel = true;
        }
    }

    public void respawn(bool fullRespawn = false) 
    { 
        _audioSource.Stop();
        _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[3]._sound);
        _spawnPoint.UpdateSpawn();
        transform.position = _spawnPoint._spawnPoint.position; 
        if(fullRespawn)_lives = 3f;
        _resetLevel = false;
        _isDead = false; 
    }

    #endregion Status

   /// <summary>
   /// Handles Unity Update Functions
   /// > FixedUpdate()
   /// </summary>
   #region Updates
private void FixedUpdate()
{
    // Handle Collisions
    HandleCollisions();
    
    // Handle Movement
    HandleMovement();
}

private void HandleCollisions()
{
    // Collisions
    groundCheck();
    ceilingCheck();
    
    // Only check walls when the player is moving
    if ( _movementInput.x != 0)
    {
        collectCheck();
        interactableCheck();
        if(!isGrounded) wallCheck();
    }
    // Hazard check can be done consistently
    hazardCheck();
}

private void HandleMovement()
{
    move();
    jump();
    //nudge();
    //assistUpward();
    //modifyApex();
    //wallSlide();
    fall();
    land();
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
        _movementInput = inputValue.Get<Vector2>();
        _isMoving = true;
        // animation
        _animator.SetFloat("xVelocity", Mathf.Abs(_movementInput.x));

        if(isGrounded)
        {
            int soundIndex = UnityEngine.Random.Range(0,1);
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[soundIndex]._sound);
        }
        // flip
        if (inputValue.Get<Vector2>().x != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(_movementInput.x), 1, 1);
        }
    }
    private void OnJump()
    {
        if( (isGrounded || _coyoteJump) && _canJump)
        { 
            _isJumping = true;
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
        {
            _startGame = true;
        }
    }

    #endregion Input

    /// <summary>
    /// Handles Walking / Running / pushing
    /// </summary>
    #region Horizontal Movement

    [Header("Horizontal Movement")]
    [SerializeField] private float _speed = 4f;
    private Vector2 _movementInput, _velocityRef;
    private bool _isMoving = false;
    private readonly float _velocityTransitionSpeed = 0.1f;
    private void move()
    {
        if(_isMoving)
        {
            _rigidbody.velocity = Vector2.SmoothDamp
            (
                _rigidbody.velocity,
                new Vector2(_movementInput.x * _speed, _rigidbody.velocity.y),
                ref _velocityRef,
                0.1f
            );
        }
    }
    private void stopMovingX()
    {
        if (_isMoving && Mathf.Abs(_rigidbody.velocity.x) <= 0.01f)
        {
        _isMoving = false; 
        }
    }
    // Generic method to smooth and clamp velocity
    private void applyVelocity(Vector2 targetVelocity, float magnitude)
    {
        _rigidbody.velocity = Vector2.ClampMagnitude
        (
            Vector2.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocityRef, _velocityTransitionSpeed),
            magnitude
        );
    }

    #endregion Horizontal Movement

    /// <summary>
    /// Handles Jumping / falling / Crouching
    /// </summary>
    #region Vertical Movement

    [Header("Vertical Movement")]
    [SerializeField] private float _jumpForce = 30f;
    [SerializeField] private float _jumpMagnitude = 20f;
    [SerializeField] private float _fallSpeed = 1.4f;
    [SerializeField] private float _fallMagnitude = 13f;
    [SerializeField] private float _nudgeSpeed = 1.1f;
    [SerializeField] private float _nudgeMagnitude = 1.3f;
    [SerializeField] private float _upAssistSpeed = 0f;
    [SerializeField] private float _upAssistMagnitude = 0f;
    private bool _isFalling = false;
    private bool _isJumping = false;
    private bool _canAssist = true;
    private void jump()
    {
        if (_isJumping) 
        {
            _canModify = true;
            _audioSource.PlayOneShot(AudioManager._instance._sfxPlayer[5]._sound);
            applyVelocity(new Vector2(_rigidbody.velocity.x, _jumpForce * Vector2.up.y),_jumpMagnitude);
            _isJumping = false;
            StartCoroutine(CanJumpDelay());
        }
    }
    private void fall()
    {
        if(!isGrounded && _rigidbody.velocity.y < 0)
        {
            _isFalling = true;
            float clampedYVelocity = Mathf.Max(_rigidbody.velocity.y, -_fallMagnitude);
            applyVelocity(new Vector2(_rigidbody.velocity.x, clampedYVelocity * _fallSpeed), _fallMagnitude);
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
        if(_rigidbody.velocity.y < 0.01f && !isGrounded)
        {
           StartCoroutine(ApexModifierDelay());
           if(_canModify)
           {
              _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
              _rigidbody.gravityScale = 0.5f;
           } 
           _rigidbody.gravityScale = 1f;
        }  
    }
    private void nudge()
    {
        if(hitCeiling && !isGrounded)
        {
            applyVelocity(new Vector2(_rigidbody.velocity.x * _nudgeSpeed, _rigidbody.velocity.y * _nudgeSpeed),_nudgeMagnitude);
        }
    }
    private void assistUpward()
    {
        if((hitRight || hitLeft) && _rigidbody.velocity.y > 0 && _canAssist) 
        {
            applyVelocity(new Vector2(_rigidbody.velocity.x * _upAssistSpeed, _rigidbody.velocity.y * _upAssistSpeed),_upAssistMagnitude);
            _canAssist = false;
        }
    } 
    private void wallSlide()
    {
        if((hitRight || hitLeft))
        {
            applyVelocity(new Vector2(0f, -(_rigidbody.velocity.y * _fallSpeed)),_fallMagnitude);
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
    private readonly float radius = 0.1f;
    private bool isGrounded, wasGrounded, hitCeiling, hitLeft, hitRight;

    private void groundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheckObj.position, radius, _levelManager.groundLayer);

        if (wasGrounded && !isGrounded)
        { 
            StartCoroutine(CoyoteJumpDelay()); 
        }
        _animator.SetBool("isJumping", !isGrounded);
    }
    private void ceilingCheck()
    {
        hitCeiling = Physics2D.OverlapCircle(ceilingCheckObj.position, 0.3f, _levelManager.groundLayer);
        platformCheck();
    }
    private void hazardCheck()
    {
      if(Physics2D.IsTouchingLayers(_collider, _levelManager.hazardLayer)  && !_isImmune)
      {
        takeDamage();
        StartCoroutine(DamageImmunity());
      }
    }
    private void wallCheck()
    {
        hitLeft = Physics2D.OverlapCircle(leftCheckObj.position, 0.3f, _levelManager.groundLayer) && !isGrounded && transform.localScale.x == -1;
        hitRight = Physics2D.OverlapCircle(rightCheckObj.position, 0.3f, _levelManager.groundLayer) && !isGrounded && transform.localScale.x == 1;
    }
    private void collectCheck()
    {
        Collider2D collider = Physics2D.OverlapCircle(_rigidbody.position, 0.5f, _levelManager.collectableLayer);
        if (collider != null)
        {
             if (collider.CompareTag("Heart"))
                {
                    _audioSource.PlayOneShot(AudioManager._instance._sfxItem[0]._sound);
                    Destroy(collider.gameObject);
                    _lives++;
                }
                else if (collider.CompareTag("Key"))
                {
                    _audioSource.PlayOneShot(AudioManager._instance._sfxItem[1]._sound);
                    Destroy(collider.gameObject);
                    _keys++;
                }
        }
    }
    private void interactableCheck()
    {
        Collider2D collider = Physics2D.OverlapCircle(_rigidbody.position, 0.5f, _levelManager.interactableLayer);
        if (collider != null)
        {
            if (collider.CompareTag("Door"))
            {
                if(_keys >= 1)
                {
                    _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[0]._sound);
                    Destroy(collider.gameObject);
                    --_keys;
                }
                else
                {
                    if (_canPlayReject)
                    {
                        _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[6]._sound);
                        StartCoroutine(PlayDoorRejection());
                    }
                }
            } 
            else if(collider.CompareTag("Push Block"))
            {
                Rigidbody2D blockRigidBody = collider.GetComponent<Rigidbody2D>();
                AudioSource blockAudioSource = collider.GetComponent<AudioSource>();
                Pushable pushableComponent = collider.GetComponent<Pushable>();

                if(blockRigidBody.velocity.x != 0  && !blockAudioSource.isPlaying && !pushableComponent.onTop)
                {
                    blockAudioSource.PlayOneShot(AudioManager._instance._sfxWorld[1]._sound);
                }
            }
        }
    }

    private void platformCheck()
    {  
        Collider2D collider = Physics2D.OverlapCircle(ceilingCheckObj.position, 0.3f, _levelManager.platformLayer);
        if (collider != null)
        {
            Physics2D.IgnoreLayerCollision(3, 14, true);
            StartCoroutine(PlatfromCollisonDelay());
                 
        }
        collider = Physics2D.OverlapCircle(groundCheckObj.position, 0.3f, _levelManager.platformLayer);
        if(isGrounded && _canCollidePlatform && collider != null)
        {
            Physics2D.IgnoreLayerCollision(3, 14, false);
            if(collider.CompareTag("Platform") && collider.GetComponent<Moveable>()._isHorizontal)
            {
                _rigidbody.velocity = new Vector2 (collider.GetComponent<Rigidbody2D>().velocity.x * 1.15f, _rigidbody.velocity.y);
            }  
            MoveableTrigger moveableTrigger = collider.GetComponent<MoveableTrigger>();
            if(collider.CompareTag("TriggerPlatform") && collider.GetComponent<MoveableTrigger>()._isHorizontal)
            {
                _rigidbody.velocity = new Vector2 (collider.GetComponent<Rigidbody2D>().velocity.x * 1.15f, _rigidbody.velocity.y);
            } 
        }
    }
   
    #endregion Collisions

    /// <summary>
    /// Generalized coroutine Function
    /// </summary>
    #region Coroutines

    [Header("Coroutines")]
    [SerializeField] private float _coyoteTime = 0.3f;
    private bool _coyoteJump = false;
    private IEnumerator CoyoteJumpDelay()
    {
        _coyoteJump = true;
        yield return new WaitForSeconds(_coyoteTime);
        _coyoteJump = false;
    }
    [SerializeField] private float _jumpDelay = 0.4f;
    private bool _canJump = true;
    private IEnumerator CanJumpDelay()
    {
        _canJump = false;
        yield return new WaitForSeconds(_jumpDelay);
        _canJump = true;
    }
    [SerializeField] private float _immunityTime = 2f;
    private bool _isImmune = false;
     private IEnumerator DamageImmunity()
    {
        _isImmune = true;
        yield return new WaitForSeconds(_immunityTime);
        _isImmune = false;
    }
    [SerializeField] private float _apexModifier = 0f;
    private bool _canModify = true;
    private IEnumerator ApexModifierDelay()
    {
        _canModify = true;
        yield return new WaitForSeconds(_apexModifier);
        _canModify = false;
    }
    [SerializeField] private float _platformCollisionDelay = 1f;
    private bool _canCollidePlatform;
    private IEnumerator PlatfromCollisonDelay()
    {
        _canCollidePlatform = false;
        yield return new WaitForSeconds(_platformCollisionDelay);
        _canCollidePlatform = true;
    }

    [SerializeField] private float _doorRejctionDelay = 3f;
    private bool _canPlayReject = true;

    private IEnumerator PlayDoorRejection()
    {
        _canPlayReject = false;
        yield return new WaitForSeconds(_doorRejctionDelay);
        _canPlayReject = true;
    }

    #endregion Coroutines

}

