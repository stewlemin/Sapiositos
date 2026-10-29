using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MOVER : MonoBehaviour
{
    public Transform cam;              // drag your camera here
    public float swimSpeed = 5f;
    public float verticalSpeed = 3f;
    public float lookSpeed = 0.1f;

    Rigidbody rb;
    float yaw, pitch;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;         // no gravity underwater
#if UNITY_6000_0_OR_NEWER
        rb.linearDamping = 3f;         // water resistance
#else
        rb.drag = 3f;
#endif
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        if (cam == null && Camera.main != null) cam = Camera.main.transform;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Mouse look
        Vector2 look = Mouse.current.delta.ReadValue() * lookSpeed;
        yaw += look.x;
        pitch = Mathf.Clamp(pitch - look.y, -89f, 89f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        cam.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void FixedUpdate()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float v = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        float up = (kb.spaceKey.isPressed ? 1f : 0f) - (kb.leftCtrlKey.isPressed ? 1f : 0f);

        // Forward follows where the camera looks, so looking down swims down
        Vector3 dir = cam.forward * v + cam.right * h;
        Vector3 force = dir * swimSpeed + Vector3.up * up * verticalSpeed;

        rb.AddForce(force, ForceMode.Acceleration);
    }
}