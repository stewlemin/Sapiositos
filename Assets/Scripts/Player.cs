using UnityEngine;
using UnityEngine.InputSystem; 

public class Player : MonoBehaviour
{
    
    public Transform camerapov;//player's camera perspective
    float camerapitch;//vertical angle of camera
    public float moveSpeed = 8f;            
    public float lookSensitivity = 0.1f;   
    private CharacterController controller;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        //lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   //mous emovement
        Vector2 look = Mouse.current.delta.ReadValue() * 0.1f;
        //rotate player based on mouse
        transform.Rotate(0, look.x, 0);
        //camera pitch based on mouse
        camerapitch = Mathf.Clamp(camerapitch - look.y, -90, 90); 
        //camera rotation
        camerapov.localRotation = Quaternion.Euler(camerapitch, 0, 0); 

        float x = 0; 
        float z = 0; 

        if (Keyboard.current.dKey.isPressed) x += 1; 
        if (Keyboard.current.aKey.isPressed) x -= 1; 
        if (Keyboard.current.wKey.isPressed) z += 1; 
        if (Keyboard.current.sKey.isPressed) z -= 1; 

        //movement direction vector
        Vector3 move = (transform.right * x + transform.forward * z).normalized * moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }
}
