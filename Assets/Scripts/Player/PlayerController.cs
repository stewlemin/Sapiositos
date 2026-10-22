using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Inspector exposed variables 
    [SerializeField] GameObject mainCamera;
    //temp prototype variables move later intro scriptable objects???
    //maybe change location later
    [SerializeField] float lookSensitivity;
    [SerializeField] float maxCameraRotation;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float acceleration = 2;
    [SerializeField] float verticalSway;
    
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

    
    //Initializations
    //
    //movement realted
    private CharacterController controller;
    private Vector2 horizontalDirection;
    private Vector3 currentVelocity;

    //camera realted
    private Vector2 cameraDisplacement;
    private float pitch = 0f;
    private float yaw = 0f;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    void OnEnable(){
	UserInput.moveEvent += OnMove;
	UserInput.lookEvent += OnLook;
	UserInput.moveCanceledEvent += OnMoveStop;
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
	handleCamera(cameraDisplacement);
    }

    private void handleLandMovement(){	
	Vector3 inputDirection = new Vector3(horizontalDirection.x, 0, horizontalDirection.y); 
	Vector3 targetDirection = transform.rotation * inputDirection;
	Vector3 targetVelocity = targetDirection * targetSpeed;
	currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

	controller.Move(currentVelocity * Time.deltaTime);

	


		

    }

    private void handleCamera(Vector2 mouseDelta){
	pitch -= mouseDelta.y * lookSensitivity;
	pitch = Math.Clamp(pitch,-maxCameraRotation, maxCameraRotation);
	yaw += mouseDelta.x * lookSensitivity;
	transform.rotation = Quaternion.Euler(pitch,yaw,0);
    }

    //reading and storing the values of the input events
    private void OnMove(Vector2 direction){
	    horizontalDirection = direction;
	    targetSpeed = maxSpeed; 
    }

    private void OnLook(Vector2 displacement){
	cameraDisplacement = displacement; 
    }


    private void OnMoveStop(Vector2 directionZero){
    //	horizontalDirection = directionZero;
    	targetSpeed = 0f;
    }


}
