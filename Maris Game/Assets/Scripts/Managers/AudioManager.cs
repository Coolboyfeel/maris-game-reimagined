using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour, IDataPersistence
{
    public bool subtitles = false;
    public Sound[] sounds;

    private void Awake() {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    //Plays a sound with given name on the standard source
    public void PlaySound(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s != null) {
            s.source.Play();
        }
    }

    //Plays a sound with given name on given source 
    public void Play(string name, AudioSource source) {
        AudioClip s = FindClip(name);
        source.clip = s;
        source.Play();
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
    public AudioClip FindClip(string name)  {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        
        if(s == null) {
            throw new Exception("Audio clip: " + name + " not found");
        }
        

        return s.source.clip;
    }

    public SubtitleObject FindSubtitle(string name) {
        if(!subtitles) {
            return null; 
        }
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s != null) {

            if(GameManager.instance.language == "English") {
                return s.subtitleEnglish;
            } else if(GameManager.instance.language == "Nederlands") {
                return s.subtitleDutch;
            } else {
                return null;
            }
        }
        Debug.LogError("Associated Subtitle: " + name + " not found");
        return null;
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

    public void OnSceneLoaded() {

    }

}
