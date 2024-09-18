using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private PreasurePlate _trigger;
    private AudioSource _audioSource;
    public float _offset;
    public bool isHorizontal;
    public bool goUp;
    public bool goLeft;
    private Vector2 _origin;
    private Vector2 _velRef;

    private bool _isMoving;  // Track if the barrier is moving
    private Vector2 _targetPosition;  // Store the target position for comparison

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _origin = transform.position;
        _targetPosition = _origin;  // Initialize target position to origin
    }

    void Update()
    {
        // Determine the target position based on the trigger and direction
        if (_trigger.isTriggered)
        {
            if (!isHorizontal) // Vertical movement
            {
                if (!goUp) // Moving down
                {
                    _targetPosition = new Vector2(_origin.x, _origin.y + -_offset);
                }
                else // Moving up
                {
                    _targetPosition = new Vector2(_origin.x, _origin.y + _offset);
                }
            }
            else // Horizontal movement
            {
                if (!goLeft) // Moving left
                {
                    _targetPosition = new Vector2(_origin.x + -_offset, _origin.y);
                }
                else // Moving right
                {
                    _targetPosition = new Vector2(_origin.x + _offset, _origin.y);
                }
            }
        }
        else // Return to origin
        {
            _targetPosition = _origin;
        }

        // Move the barrier to the target position
        transform.position = Vector2.SmoothDamp
        (
            transform.position,
            _targetPosition,
            ref _velRef,
            0.25f
        );

        // Check if the barrier is moving
        _isMoving = Mathf.Abs(Vector2.Distance(transform.position, _targetPosition)) > 0.01f;

        // Play or stop sound based on movement
        if (_isMoving && !_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(AudioManager._instance._sfxWorld[8]._sound);  // Start playing the sound when moving
        }
        else if (!_isMoving && _audioSource.isPlaying)
        {
            _audioSource.Stop();  // Stop the sound when the barrier stops moving
        }
    }
}
