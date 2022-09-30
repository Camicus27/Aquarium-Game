using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerGroup soundMixer;
    public AudioMixerGroup musicMixer;
    public Sound[] sounds;
    public Sound[] music;
    private Dictionary<string, Sound> soundsSet;
    private Dictionary<string, Sound> musicSet;
    public Sound currentSong;

    [HideInInspector]
    public bool soundMute;
    [HideInInspector]
    public bool musicMute;

    void Awake()
    {
        soundsSet = new Dictionary<string, Sound>();
        musicSet = new Dictionary<string, Sound>();

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.outputAudioMixerGroup = soundMixer;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            soundsSet.Add(sound.name, sound);
        }

        foreach (Sound music in music)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.clip;
            music.source.outputAudioMixerGroup = musicMixer;
            music.source.volume = music.volume;
            music.source.pitch = music.pitch;
            music.source.loop = music.loop;

            musicSet.Add(music.name, music);
        }
    }

    public string GetCurrentlyPlayingSong()
    {
        return currentSong.name;
    }

    public void PlaySound(string soundName)
    {
        Sound sound;

        sound = soundsSet[soundName];

        if (sound.source.isPlaying)
            sound.source.Stop();
        sound.source.Play();
    }

    public void PlayMusic(string songName)
    {
        Sound song;

        song = musicSet[songName];
        currentSong = song;

        if (song.source.isPlaying)
            song.source.Stop();
        song.source.Play();
    }

    public void StopSongSlowly(string songName, float fadeOutTime)
    {
        Sound song = musicSet[songName];
        StartCoroutine(Fade(song, fadeOutTime, song.source.volume));
    }
    private IEnumerator Fade(Sound song, float fadeTime, float ogVolume)
    {
        float alpha;
        alpha = ogVolume;
        while (alpha > 0)
        {
            alpha -= 0.005f / fadeTime;
            song.source.volume = alpha;
            yield return null;
        }
        song.source.Stop();
        song.source.volume = ogVolume;
    }

    public void ToggleMuteSound()
    {
        if (soundMute)
        {
            soundMute = false;
            soundMixer.audioMixer.SetFloat("SoundVolume", 0f);
        }
        else
        {
            soundMute = true;
            soundMixer.audioMixer.SetFloat("SoundVolume", -80f);
        }
    }

    public void ToggleMuteMusic()
    {
        if (musicMute)
        {
            musicMute = false;
            musicMixer.audioMixer.SetFloat("MusicVolume", 0f);
        }
        else
        {
            musicMute = true;
            musicMixer.audioMixer.SetFloat("MusicVolume", -80f);
        }
    }
}
