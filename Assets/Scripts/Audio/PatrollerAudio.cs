using UnityEngine;

[RequireComponent(typeof(SpatialAudioManager))]
public class PatrollerAudio : MonoBehaviour
{
    [SerializeField] MonsterEventChannel events;
    public SoundVariations warningClip;
    public SoundVariations redlightClip;
    [SerializeField] bool stopAfterRed = true;
    SpatialAudioManager spatial;

    void Awake()
    {
        spatial = GetComponent<SpatialAudioManager>();
    }

    void OnEnable()
    {
        events.OnWarningStarted += HandleWarningStarted;
        events.OnSoundStarted += HandleSoundStarted;
        events.OnSoundEnded += HandleSoundEnded;
        events.OnJumpscareTriggered += HandleJumpscare;
    }

    void OnDisable()
    {
        events.OnWarningStarted -= HandleWarningStarted;
        events.OnSoundStarted -= HandleSoundStarted;
        events.OnSoundEnded -= HandleSoundEnded;
        events.OnJumpscareTriggered -= HandleJumpscare;
    }

    void PlayVariation(SoundVariations sounds)
    {
        if (sounds == null) return;
        AudioClip clip = sounds.getSound();
        if (clip != null) spatial.play3DSFX(clip);
    }

    void HandleWarningStarted() => PlayVariation(warningClip);
    void HandleSoundStarted() => PlayVariation(redlightClip);
    void HandleSoundEnded()
    {
        if (stopAfterRed) spatial.stop3DSFX();
    }
    void HandleJumpscare() => spatial.stop3DSFX();

}
