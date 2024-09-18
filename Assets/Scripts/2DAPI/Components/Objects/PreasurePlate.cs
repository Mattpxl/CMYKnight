using UnityEngine;

public class PreasurePlate : MonoBehaviour
{
    private LayerManager _levelManager;
    private Collider2D _collider;
    private Animator _animator;
    private AudioSource _audioSource;
    public bool isTriggered = false;
    void Awake()
    {
        _levelManager = GetComponent<LayerManager>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        CollisionCheck();
    }
    private void CollisionCheck()
    {
        if(Physics2D.IsTouchingLayers(_collider, _levelManager.playerLayer) || Physics2D.IsTouchingLayers(_collider, _levelManager.interactableLayer))
        {
            _animator.SetBool("isTriggered", isTriggered);
            if(!_audioSource.isPlaying && !isTriggered)_audioSource.PlayOneShot(AudioManager._instance._sfxWorld[7]._sound);
            isTriggered = true;
        }
        else 
        {
            if(isTriggered)
            {
                isTriggered = false;
                _animator.SetBool("isTriggered", isTriggered);
                if(!_audioSource.isPlaying)_audioSource.PlayOneShot(AudioManager._instance._sfxWorld[7]._sound);
            }
        }

    }
}
