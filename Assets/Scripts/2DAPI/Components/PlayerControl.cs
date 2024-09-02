using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DataStorage;
using System.Runtime.CompilerServices;
using System;

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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
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
    private String[] _valueNames;
    private String[] _valueTypes;
    private ValueTable _playerData;
    private TableEntry[] _playerDataSet;
    private TableEntry[] _playerAccessDataSet;
    private static TableEntry _playerTableID;
    private static TableEntry _playerLevel, _playerLevelDefault;
    private static TableEntry _playerLives, _playerLivesDefault;
    private static TableEntry _playerKeys, _playerKeysDefault;
     private void initializeTable()
    {
        _playerData = new ValueTable
        (
            "PlayerData",
            _valueNames = new String[3]
            {
                "Level",
                "Lives", 
                "Keys",
            },
            _valueTypes = new String[3]
            {
                "INT",
                "REAL", 
                "REAL",
            }
        );
        _sqlConverter.generateTable(_playerData);

        // PRIMARY KEY ACCESS
        _playerTableID = new TableEntry
        (
            "PlayerData_id",
            "INT",
            1
        );
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

        _playerLevelDefault = new TableEntry
        (
            "Level",
            "INT",
            1
        );
        _playerLivesDefault = new TableEntry
        (
            "Lives",
            "REAL",
            3
        );
        _playerKeysDefault = new TableEntry
        (
            "Keys",
            "REAL",
            0
        );
        _playerAccessDataSet = new TableEntry[4]
        {
            _playerTableID,
            _playerLevelDefault,
            _playerLivesDefault,
            _playerKeysDefault
        };
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
        _level = loadPlayerValue(_playerLevel)._intValue;
        _lives = loadPlayerValue(_playerLives)._floatValue;
        _keys = loadPlayerValue(_playerKeys)._floatValue;
    }
    private void OnApplicationQuit()
    {
        _sqlConverter.updateTable(_playerData, _playerDataSet, _playerTableID);
    }

   #endregion Data

    /// <summary>
    /// Keep Track of Player Stats and Health
    /// </summary>
    #region Status

    [Header("Status")]
    private int _level = 0; // get from scene manager 
    [SerializeField] private float _lives;
    [SerializeField] private float _keys;
    private bool _isDead = false;

    private void takeDamage() 
    { 
        if(_lives >= 1) {
            --_lives;
            isGrounded = _rigidbody.velocity.y <= 0;
        }
        else if (_lives <= 0) 
        {
            _isDead = true; 
            _lives = 0f; 
        } 
        if(_isDead) respawn();
    }
    private void respawn() 
    { 
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
        groundCheck();
        ceilingCheck();
        collectCheck();
        hazardCheck();
        interactableCheck();
        move();
        if (_isJumping) jump();
        // if(!_isJumping) crouch();
        // if (_isJumping && !_isCrouching) jump(); 
    }
    #endregion Updates
    
    /// <summary>
    /// Recieve Input from Unity Input System
    /// </summary>
    #region Input
    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>() == Vector2.zero ? Vector2.zero : inputValue.Get<Vector2>();
        // animation
        _animator.SetFloat("xVelocity", inputValue.Get<Vector2>().x == -1f ? 1 : inputValue.Get<Vector2>().x);

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
        if (isGrounded || _coyoteJump) _isJumping = true; 
        // variable jump height - add downward force when player rceleases jump
        // apex modifiers - at the apex of the jump there is a moment of anti-gravity & maybe minor speed boost
        // jump buffering - queue up next jump before htting the ground
        // clamped fall speed - allows landing on platforms while falling easier
        // edge detection for ceilings - move player slightly over and continue upward motion
        // catch edges when jumping on platfroms and nudge up
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
    private bool _isJumping = false;
    private void jump()
    {
       // _rigidbody.velocity += _jumpForce * Vector2.up;
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
    private ContactFilter2D _layerFilterCollect, _layerFilterInteract;
    private const float radius = 0.2f;
    private bool isGrounded, wasGrounded, hitCeiling;

    private void groundCheck()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircleAll(groundCheckObj.position, radius, _levelManager.groundLayer).Length > 0 ? true : false;
        if (wasGrounded) StartCoroutine(CoyoteJumpDelay());
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
        Debug.Log(_lives);
      }
    }
    private void collectCheck()
    {
        if (Physics2D.IsTouchingLayers(_collider, _levelManager.collectableLayer))
        {
            _layerFilterCollect.SetLayerMask(_levelManager.collectableLayer);
            Collider2D[] colliders = new Collider2D[1];
            Physics2D.GetContacts(_collider, _layerFilterCollect, colliders);

        
            if (colliders[0].gameObject.CompareTag("Heart"))
            {
                Destroy(colliders[0].gameObject);
                ++_lives;
            }
            else if (colliders[0].gameObject.CompareTag("Key"))
            {
                Destroy(colliders[0].gameObject);
                ++_keys;
            }
        }
    }
    private void interactableCheck()
    {
         if (Physics2D.IsTouchingLayers(_collider, _levelManager.interactableLayer))
        {
            _layerFilterInteract.SetLayerMask(_levelManager.interactableLayer);
            Collider2D[] colliders = new Collider2D[1];
            Physics2D.GetContacts(_collider, _layerFilterInteract, colliders);

            if (colliders[0].gameObject.CompareTag("Door") && _keys >= 1)
            {
                Destroy(colliders[0].gameObject);
                --_keys;
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

