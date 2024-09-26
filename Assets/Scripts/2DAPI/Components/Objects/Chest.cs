using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Timeline.Actions;
using Unity.VisualScripting;

public class Chest : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audioSource;
    private CircleCollider2D _circleCollider;
    private BoxCollider2D _boxCollider;
    public bool yellow;
    public bool cyan;
    public bool magenta;
    public bool isOpen;
    public bool isOpened;
    public string _loot;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        isOpen = false;
        isOpened = false;
        _loot = GetLoot();
    }

    private void FixedUpdate()
    {
        if(isOpen)
        {
            //if(!_audioSource.isPlaying) _audioSource.PlayOneShot(AudioManager._instance._sfxItem[3]._sound);
            _animator.SetBool("isOpening", true);
            _boxCollider.enabled = true;
            if(!_audioSource.isPlaying)
            {
                if(yellow)
                {
                    if(!_audioSource.isPlaying) _audioSource.PlayOneShot(AudioManager._instance._sfxItem[4]._sound);
                }
                else if(cyan)
                {
                    if(!_audioSource.isPlaying) _audioSource.PlayOneShot(AudioManager._instance._sfxItem[0]._sound);
                }
                else if(magenta)
                {
                    if(!_audioSource.isPlaying) _audioSource.PlayOneShot(AudioManager._instance._sfxItem[5]._sound);
                }
            }
            _animator.SetBool("isOpen", true);
            _circleCollider.enabled = false;
            StartCoroutine(WaitToPrevent());
        }
    }

    public List<string> lootFlags = new() 
    {
        "heart",
        "coins",
        "steal",
        "key",
        "cKey",
        "mkey",
        "sKey"
    };

    public string GetLoot()
    {
            if(yellow)
            {
                return "coins";
            }
            else if(cyan)
            {
                return "heart";
            }
            else if(magenta)
            {
                return "steal";
            }
            else 
            {
                Debug.Log("Chest didn't give loot");
                return "";
            }
            
    }

    private IEnumerator WaitToPrevent()
    {
        yield return new WaitForSeconds(2);
        isOpen = false;
    }
}
