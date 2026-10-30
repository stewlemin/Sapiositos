// using UnityEngine;
// using UnityEngine.InputSystem;

// public class FakePickup : MonoBehaviour
// {
//     public MonsterEventChannel events;

//     void Update()
//     {

//         if (Keyboard.current.digit1Key.wasPressedThisFrame)
//         {
//             events.BroadcastArtefactCollected(1);
//             Debug.Log("artefact 1 collected");
//         }
//         else if (Keyboard.current.digit2Key.wasPressedThisFrame)
//         {
//             events.BroadcastArtefactCollected(2);
//             Debug.Log("artefact 2 collected");
//         }
//     }
// }