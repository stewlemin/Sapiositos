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

    //SpatialAudioManager spatial;
    AudioManager audioManager;

    void Awake()
    {
        audioManager = GetComponent<AudioManager>();
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

    void HandleGazeStarted() => AudioManager.instance.playSFX(clip1);
    void HandleGazeEnded() => AudioManager.instance.playSFX(clip2);
}
