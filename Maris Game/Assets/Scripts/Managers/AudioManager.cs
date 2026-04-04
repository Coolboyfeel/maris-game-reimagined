using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour, IDataPersistence
{
    public bool subtitles = false;
    public Sound[] sounds;

    private GameManager gameM;
    private AudioSource defaultSource;

    public float volume = 1f;

    private void Awake() {
        /*
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        } */

        defaultSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        gameM = GameManager.instance;
    }

    //Plays a sound with given name on the standard source
    public void Play(string name) {
        Sound s = FindSound(name);
        Play(s, defaultSource);
    }

    //Plays a sound with given name on given source 
    public void Play(string name, AudioSource source) {
        Sound s = FindSound(name);
        Play(s, source);
    }
    
    //Plays a given sound on given source
    public void Play(Sound sound, AudioSource source)
    {
        //Check if the source is not already playing the given sound.
        if (source.isPlaying) {
            if (source.clip == sound.clip) {
                return;
            }
            else {
                Stop(source);    
            }
            
        } 
        if (source.isPlaying) Stop(source);
        InitializeSource(sound, source);
        source.Play();
    }

    private void InitializeSource(Sound sound, AudioSource source)
    {
        source.clip = sound.clip;
        source.volume = sound.volume * this.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.spatialBlend = sound.spatialBlend;
    }


    //Stops a source playing a sound with a check if it's playing given sound
    public void Stop(string name, AudioSource source) {
        if(source.isPlaying && source.clip.name == name) {
            source.Stop();
        } 
    }

    //Stops a source without any checks.
    public void Stop(AudioSource source) {
        if(source.isPlaying) {
            source.Stop();
        }
    }

     
    /*
    * Finds a clip in the array with given name
    * Throws exception if clip not found
    */
    public Sound FindSound(string name)  {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        if(s == null) {
            throw new Exception("Sound object: " + name + " not found");
        }
        
        return s;
    }

    public SubtitleObject FindSubtitle(string name) {
        if(!subtitles) {
            return null; 
        }
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) {
            throw new Exception("Associated Subtitle: " + name + " not found");
        }
        
        switch (GameManager.instance.language) {
            case "English":
                return s.subtitleEnglish;
            case "Nederlands":
                return s.subtitleDutch;
            default:
                return null;
        }
    }

    public void StopAllSounds() {
        foreach (Sound s in sounds) {
            s.source.Stop();
        }
    }

    public void LoadData(GameData data) {
        subtitles = data.subtitles;
    }

    public void SaveData(ref GameData data) {

    }

    public void OnSceneLoaded()
    {
        string curScene = SceneManager.GetActiveScene().name;
        if (curScene == "Maris" || curScene == "Game Over")
        {
            Play("Ambience");
        }
    }

}
