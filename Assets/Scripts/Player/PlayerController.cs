using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Enum conveying the player's environmental state
    enum EnvironmentType{
	Water,
	Land
    }

    //Movement realted states undewater
    enum WaterHorizontal{
        Idle,
	Swim
    }

    enum WaterVertical{
	Idle,
	Ascend,
	Descend
    }

    //Movement related states on land
    enum LandHorizontal{
        Idle,
	Walking,
	Sprinting,
    }

    enum LandVertical{
	Idle,
	Jumping,
	Falling
    }
    
    //initializations
    private CharacterController controller;
    private Vector2 horizontalDirection;

    void OnEnable(){
	UserInput.moveEvent += OnMove;
    }

    void Awake()
    {
	controller = gameObject.GetComponent<CharacterController>();
    }

    void Start()
    {
	Cursor.visible = false;
	Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
	handleLandMovement();
	handleCamera();
    }

    private void handleLandMovement(){	
	Vector3 movementVector = new Vector3(horizontalDirection.x, 0, horizontalDirection.y);
	float arbitrarySpeed = 2.0f; //to be changed to the PlayerStats Speed

	controller.Move(movementVector * arbitrarySpeed * Time.deltaTime);

	


		

    }

    private void handleCamera(){
	    

    }

    private void OnMove(Vector2 direction){
	horizontalDirection = direction;
    }
}
