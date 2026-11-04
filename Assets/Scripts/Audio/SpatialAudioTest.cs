using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpatialAudioManager))]
public class SpatialAudioTestRig : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    SpatialAudioManager spatial;

    void Awake()
    {
        spatial = GetComponent<SpatialAudioManager>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.eKey.isPressed) spatial.play3DSFX(clip);
    }
}
