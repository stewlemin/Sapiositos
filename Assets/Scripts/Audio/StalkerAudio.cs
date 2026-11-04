using UnityEngine;

// Listens to the monster's event channel and turns events into sounds
[RequireComponent(typeof(SpatialAudioManager))]
public class StalkerAudio : MonoBehaviour
{
    [SerializeField] MonsterEventChannel events;

    [Header("3D sound variations")]
    public SoundVariations gazeStartedSounds;

    SpatialAudioManager spatial;
    //AudioManager audioManager;

    void Awake()
    {
        //audioManager = GetComponent<AudioManager>();
        spatial = GetComponent<SpatialAudioManager>();
    }

    void OnEnable()
    {
        events.OnGazeStarted += HandleGazeStarted;
    }

    void OnDisable()
    {
        events.OnGazeStarted -= HandleGazeStarted;
    }

    void PlayVariation(SoundVariations variations)
    {
        if (variations == null) return;
        AudioClip clip = variations.getSound();

        if (clip != null) spatial.play3DSFX(clip);
    }

    void HandleGazeStarted() => PlayVariation(gazeStartedSounds);
}
