using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 작성하는데 15분 걸렸다
public class AudioPlayerScript : MonoBehaviour
{

    public AudioPlayer player;
    public AudioSource source;
    public bool isPlaying;

    private void Update()
    {
        if (isPlaying && !source.isPlaying)
        {
            isPlaying = false;
            if (player.audioStop != null)
                player.audioStop.Invoke(gameObject, source);
        }
    }

}

public class AudioPlayer
{
    public delegate void MOnAudioPlay(GameObject player, AudioSource source);
    public delegate void MOnAudioStop(GameObject player, AudioSource source);

    private static readonly MOnAudioStop DefaultAudioStop = (player, source) => Object.Destroy(player);

    AudioClip clip;

    bool isDontDestroyOnLoad = false;

    int priority = 128;
    float volume = 1;
    float pitch = 1;
    float time = 0;

    public MOnAudioPlay audioPlay;
    public MOnAudioStop audioStop = DefaultAudioStop;

    public AudioPlayer(AudioClip clip)
    {
        this.clip = clip;
    }

    public AudioSource Play()
    {
        GameObject player = Build();
        AudioSource source = player.GetComponent<AudioSource>();
        AudioPlayerScript script = player.GetComponent<AudioPlayerScript>();
        if (audioPlay != null)
            audioPlay.Invoke(player, source);
        source.Play();
        script.isPlaying = true;
        return source;
    }

    public GameObject Build()
    {
        GameObject player = new("Audio Player");
        player.transform.SetPositionAndRotation(new(), new());
        AudioSource source = player.AddComponent<AudioSource>();
        source.clip = clip;
        source.priority = priority;
        source.volume = volume;
        source.pitch = pitch;
        source.time = time;
        if (isDontDestroyOnLoad)
            Object.DontDestroyOnLoad(player);
        AudioPlayerScript script = player.AddComponent<AudioPlayerScript>();
        script.player = this;
        script.source = source;
        return player;
    }

    public AudioPlayer DontDestroyOnLoad()
    {
        isDontDestroyOnLoad = true;
        return this;
    }

    public AudioPlayer Priority(int priority)
    {
        this.priority = priority;
        return this;
    }

    public AudioPlayer Volume(float volume)
    {
        this.volume = volume;
        return this;
    }

    public AudioPlayer Pitch(float pitch)
    {
        this.pitch = pitch;
        return this;
    }

    public AudioPlayer Time(float time)
    {
        this.time = time;
        return this;
    }

    public AudioPlayer OnAudioPlay(MOnAudioPlay audioPlay)
    {
        this.audioPlay = audioPlay;
        return this;
    }

    public AudioPlayer OnAudioStop(MOnAudioStop audioStop)
    {
        this.audioStop = audioStop;
        return this;
    }

}
