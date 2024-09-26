using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Initialization
        private LayerManager _levelManager;
        private Rigidbody2D _rigidbody;
        private CapsuleCollider2D _body;
        private BoxCollider2D _head;
        private Animator _animator;
        private AudioSource _audioSource;
        private AwarenessController _awarenessController;
        private Camera _camera;
        private PlayerControl _playerControl;

        public List<GameObject> _loot;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _body = GetComponent<CapsuleCollider2D>();
        _head = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _levelManager = GetComponent<LayerManager>();
        _awarenessController = GetComponent<AwarenessController>();
        _audioSource = GetComponent<AudioSource>();
        _playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Start()
    {
        _audioSource.Stop();
        Physics2D.IgnoreLayerCollision(10,10,true);
        _awarenessController.setTarget(_playerControl.transform.position);
    }

    #endregion Initialization

    #region Updates
    private Vector2 _velRef;  
    void FixedUpdate()
    {
        //playerCheck();
        if(_awarenessController.canCheck)
        _awarenessController.awarenessCheck();
        if(_awarenessController.isAware)
        {
            transform.localScale *= new Vector2(-GameObject.FindGameObjectWithTag("Player").transform.localScale.x, 1); 
            _audioSource.PlayOneShot(AudioManager._instance._sfxEnemy[2]._sound);
            _awarenessController.isAware = false;
            StartCoroutine(_awarenessController.AwarenessDelay());
        }
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
        muteOffScreen();
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
        if(move == 1)
        switch(Random.Range(0,1)) 
        { 
            case 0: if(_audioSource.isPlaying == false)_audioSource.PlayOneShot(AudioManager._instance._sfxEnemy[0]._sound); break; 
            case 1: if(_audioSource.isPlaying == false)_audioSource.PlayOneShot(AudioManager._instance._sfxEnemy[1]._sound); break; 
            default: break;
        } else _audioSource.Stop();
            return dir * (move > 0 ? 1 : 0); 
    }
    private void muteOffScreen()
    {
         Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);
        if 
        (
            screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight
        )   
        {
            _audioSource.mute = true;
        }
        else 
        {
            _audioSource.mute = false;
        }
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
            Physics2D.OverlapCircle(wallCheck0.position, radius, _levelManager.groundLayer) ||
            Physics2D.OverlapCircle(wallCheck1.position, radius, _levelManager.groundLayer)
        )
        {
            transform.localScale *= new Vector2(-1, 1); 
            StartCoroutine(wallCheckTimer());
            StartCoroutine(moveTimer());
            _animator.SetFloat("Move", 1);
            _rigidbody.velocity = Vector2.SmoothDamp
                (
                    _rigidbody.velocity,
                    new Vector2(_speed / 10 * _rigidbody.velocity.x, 0f),
                    ref _velRef,
                    0.1f
                );
            _animator.SetFloat("Move", 0);
        }
        }
    }

    private void playerCheck()
    {
        if(_head.IsTouchingLayers(_levelManager.playerLayer))
        {
            // sound
            // animation
            configureLoot();
        }
    }
    public void configureLoot()
    {
        // randomize
        float offset = Random.Range(-0.3f,0.3f);
        int loot = Random.Range(0,4);
        switch(loot)
        {
            case 0: //heart
                Instantiate(_loot[0], new Vector2(transform.position.x + offset, transform.position.y + 0.2f), Quaternion.identity);
            break;
            case 1: 
            case 2: 
            case 3: 
            case 4:
                int coins = Random.Range(1,3);
                for(int i = 1; i < coins; i++)
                {
                    offset = Random.Range(-0.7f,0.7f);
                    float yOffset = Random.Range(0.2f,0.4f);
                    Instantiate(_loot[1], new Vector2(transform.position.x + offset, transform.position.y + yOffset), Quaternion.identity);
                }
            break;
        }
        Destroy(gameObject);
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
