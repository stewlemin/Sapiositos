using UnityEngine;

// Listens to the monster's event channel and turns events into sounds
[RequireComponent(typeof(SpatialAudioManager))]
public class StalkerAudio : MonoBehaviour
{
    [SerializeField] MonsterEventChannel events;

    [Header("3D clips")]
    public AudioClip clip1;   
    public AudioClip clip2;   
    public AudioClip clip3;

    SpatialAudioManager spatial;
    AudioManager audioManager;

    void Awake()
    {
        spatial = GetComponent<SpatialAudioManager>();
        //audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void OnEnable()
    {
        events.OnGazeStarted += HandleGazeStarted;
        events.OnGazeEnded += HandleGazeEnded;
    }

    void OnDisable()
    {
        events.OnGazeStarted -= HandleGazeStarted;
        events.OnGazeEnded -= HandleGazeEnded;
    }

    void HandleGazeStarted() => spatial.play3DSFX(clip1);
    void HandleGazeEnded() => spatial.play3DSFX(clip2);
}
