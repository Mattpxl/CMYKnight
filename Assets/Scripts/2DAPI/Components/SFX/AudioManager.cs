using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioMixer _audioMixer;
    public AudioMixerGroup[] _audioMixerGroup;
    public static AudioManager _instance;
    public Sound[] _sfx, _sfxUI,_sfxPlayer,_sfxWorld,_sfxEnemy, _sfxItem, _sfxEnvironment, _music;
    public AudioSource _sfxSource, _musicSource;
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
       // _audioMixerGroup = _audioMixer.FindMatchingGroups("Master/Sounds/");
       // _audioMixer.outputAudioMixerGroup = _audioMixerGroup[0];
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
    public void playSfx(string name)
    {
        Sound s = Array.Find(_sfx, x=>x._name==name);
        if(s == null) Debug.Log("Sound Unavailable.");
        else
        { 
            _sfxSource.PlayOneShot(s._sound);
        }
    }
     public void toggleMusic()
    {
        _musicSource.mute = _isMasterMute? true : !_musicSource.mute;
    }
    public void toggleSfx()
    {
        _sfxSource.mute = _isMasterMute? true : !_sfxSource.mute;
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
        _sfxSource.volume = volume;
    }
    public void masterVolume(float volume)
    {
        _sfxSource.volume = _sfxSource.volume - (1-volume) <= 0 ? 0 : _sfxSource.volume - (1-volume);
        _musicSource.volume = _musicSource.volume - (1-volume) <= 0 ? 0 : _musicSource.volume - (1-volume);
    }

}
