using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpatialAudioManager : MonoBehaviour
{
    [Header("Spatial settings")]
    [SerializeField] AudioMixerGroup mixer;
    [SerializeField] float minDistance = 2f;
    [SerializeField] float maxDistance = 40f;

    AudioSource oneShotSource;
    
    readonly Dictionary<AudioClip, AudioSource> loops = new();

    void Awake(){
        oneShotSource = CreateSource();
    }

    AudioSource CreateSource(){

        var src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 1f;
        src.dopplerLevel = 0f;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.rolloffMode = AudioRolloffMode.Linear; //for patroller
        src.outputAudioMixerGroup = mixer; //for patroller
        return src;
    }

    public void play3DSFX(AudioClip clip){
        if (clip == null) return;
        oneShotSource.PlayOneShot(clip);
    }

    public void stop3DSFX()
    {
        oneShotSource.Stop();
    }
}
