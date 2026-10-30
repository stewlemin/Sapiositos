using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class RandomSound
    {
        public AudioClip clip;
        public float minInterval = 5f;   // seconds
        public float maxInterval = 15f;  // seconds
        [Range(0f, 1f)] public float chance = 1f;  // chance to actually play when the timer fires
        [Range(0f, 1f)] public float volume = 1f;
    }

    //2d AudioSources
    [Header("Audio sources")]
    [SerializeField] AudioSource backgroundSource;
    [SerializeField] AudioSource sfxSource;

    // All the audio clips that are 2d
    [Header("Audio clips")]
    public AudioClip underwaterAmbiance;
    public AudioClip monster;
    public AudioClip swimming;

    [Header("Random interval sounds")]
    [SerializeField] List<RandomSound> randomSounds = new();

    // One dedicated looping source per clip, created on demand
    readonly Dictionary<AudioClip, AudioSource> loops = new();

    void Start()
    {
        backgroundSource.clip = underwaterAmbiance;
        backgroundSource.loop = true;
        backgroundSource.Play();

        foreach (var s in randomSounds)
            StartCoroutine(RandomSoundRoutine(s));
    }

    IEnumerator RandomSoundRoutine(RandomSound s)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(s.minInterval, s.maxInterval));
            if (Random.value <= s.chance)
                sfxSource.PlayOneShot(s.clip, s.volume);
        }
    }

    // One-shot
    public void playSFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Repeating sound tied to an action
    public void startLoop(AudioClip clip, float volume = 1f){
        if (!loops.TryGetValue(clip, out var src))
        {
            src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.clip = clip;
            src.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup; // same mixer routing
            loops[clip] = src;
        }

        src.volume = volume;
        if (!src.isPlaying) src.Play();
    }

    public void stopLoop(AudioClip clip)
    {
        if (loops.TryGetValue(clip, out var src))
            src.Stop();
    }
}