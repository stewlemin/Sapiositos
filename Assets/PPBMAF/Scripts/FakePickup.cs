using UnityEngine;
using UnityEngine.InputSystem;

public class FakePickup : MonoBehaviour
{
    public MonsterEventChannel events;

    void Update()
    {

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            events.BroadcastArtefactCollected(4);
            Debug.Log("artefact collected");
        }
    }
}