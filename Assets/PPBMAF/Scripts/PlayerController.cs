using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float runningSpeed = 8.0f;

    [Header("Camera Settings")]
    [SerializeField] private Transform playerCamera; // Drag your Camera here in the Unity Inspector
    [SerializeField] private float mouseSensitivity = 0.5f;

    private float cameraVerticalAngle = 0f;

    void Start()
    {
        Debug.Log($"[Mover] Initialized on {gameObject.name}. MoveSpeed is set to {moveSpeed}.");
                Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);

            if (playerCamera != null)
            {
                cameraVerticalAngle -= mouseDelta.y * mouseSensitivity;
                cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f); 
                
                playerCamera.localEulerAngles = new Vector3(cameraVerticalAngle, 0f, 0f);
            }
        }

        float x = 0f;
        float y = 0f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.wKey.isPressed) y += 1f;
        
        Vector3 dir = new Vector3(x, 0f, y).normalized;
        float curSpeed = Keyboard.current.leftShiftKey.isPressed ? runningSpeed : moveSpeed;
        
        transform.Translate(dir * curSpeed * Time.deltaTime, Space.Self); 
    }   
}