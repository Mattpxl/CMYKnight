using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;
    public Sound[] _sfxUI,_sfxPlayer,_sfxWorld,_sfxEnemy, _sfxItem, _music;
    public AudioSource _sfxSourceEnemy, _sfxSourcePlayer, _sfxSourceUI, _sfxSourceWorld, _sfxSourceItem, _musicSource;
    private bool _isMasterMute;
    // Start is called before the first frame update
    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
    }
    void Start()
    {
        //playMusic("name"); depending on the scene [make function for it]
    }

    public void playMusic(string name)
    {
        Sound s = Array.Find(_music, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _musicSource.clip = s._sound;
            _musicSource.Play();
        }
    }
    public void playSfxUI(string name)
    {
        Sound s = Array.Find(_sfxUI, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _sfxSourceUI.PlayOneShot(s._sound);
        }
    }
     public void playSfxPlayer(string name)
    {
        Sound s = Array.Find(_sfxPlayer, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _sfxSourcePlayer.PlayOneShot(s._sound);
        }
    }
    public void playSfxWorld(string name)
    {
        Sound s = Array.Find(_sfxWorld, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _sfxSourceWorld.PlayOneShot(s._sound);
        }
    }
    public void playSfxEnemy(string name)
    {
        Sound s = Array.Find(_sfxEnemy, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _sfxSourceEnemy.PlayOneShot(s._sound);
        }
    }
    public void playSfxItem(string name)
    {
        Sound s = Array.Find(_sfxItem, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _sfxSourceItem.PlayOneShot(s._sound);
        }
    }
     public void toggleMusic()
    {
        _musicSource.mute = _isMasterMute? true : !_musicSource.mute;
    }
    public void toggleSfx()
    {
        _sfxSourcePlayer.mute = _isMasterMute? true : !_sfxSourcePlayer.mute;
        _sfxSourceWorld.mute = _isMasterMute? true : !_sfxSourceWorld.mute;
        _sfxSourceUI.mute = _isMasterMute? true : !_sfxSourceUI.mute;
        _sfxSourceEnemy.mute = _isMasterMute? true : !_sfxSourceEnemy.mute;
    }
    public void toggleMaster(bool isMute)
    {
        _isMasterMute = isMute;
    }
    public void musicVolume(float volume)
    {
        _musicSource.volume = volume;
    }
    public void sfxVolume(float volume)
    {
        _sfxSourcePlayer.volume = volume;
        _sfxSourceWorld.volume = volume;
        _sfxSourceUI.volume = volume;
        _sfxSourceEnemy.volume = volume;
    }
    public void masterVolume(float volume)
    {
        _sfxSourcePlayer.volume = _sfxSourcePlayer.volume - (1-volume) <= 0 ? 0 : _sfxSourcePlayer.volume - (1-volume);
        _sfxSourceWorld.volume = _sfxSourceWorld.volume - (1-volume) <= 0 ? 0 : _sfxSourceWorld.volume - (1-volume);
        _sfxSourceUI.volume = _sfxSourceUI.volume - (1-volume) <= 0 ? 0 : _sfxSourceUI.volume - (1-volume);
        _sfxSourceEnemy.volume = _sfxSourceEnemy.volume - (1-volume) <= 0 ? 0 : _sfxSourceEnemy.volume - (1-volume);
        _musicSource.volume = _musicSource.volume - (1-volume) <= 0 ? 0 : _musicSource.volume - (1-volume);
    }

}
