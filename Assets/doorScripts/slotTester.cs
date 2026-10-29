using UnityEngine;
using UnityEngine.InputSystem;

// Test script: press F while looking at a wall slot to move its artifact out from behind the wall.
// Put this on the player or the camera.
public class slotTester : MonoBehaviour
{
    [SerializeField] float reach = 5f;

    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.fKey.wasPressedThisFrame)
        {
            return;
        }

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, reach))
        {
            wallSlot slot = hit.collider.GetComponent<wallSlot>();
            if (slot == null)
            {
                Debug.Log("Hit " + hit.collider.name + ", not a slot");
                return;
            }

            if (!slot.Place())
            {
                Debug.Log("Slot is already filled");
            }
        }
    }
}
