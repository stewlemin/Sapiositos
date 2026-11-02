using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Inspector exposed variables 
    [SerializeField] Camera mainCamera;
    //change this to a different script later on
    [SerializeField] Animator animator;
    //temp prototype variables move later intro scriptable objects???
    //maybe change location later
    [SerializeField] float lookSensitivity;
    [SerializeField] float maxCameraRotation;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float acceleration = 2;
    [SerializeField] float interactRange = 2.0f;
    
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

    //Enum for collectable states
    enum InteractType{
	None,
	Collectible,
	Placeable
    }

    
    //Initializations
    //
    //movement realted
    private CharacterController controller;
    private Vector3 horizontalDirection;
    private Vector3 verticalDirection;
    private Vector3 currentVelocity;

    //interact related
    private bool hitInteractable = false;
    private int layerMask = 192;//6th and 7th bit are 1 coresponding to the layers that you can interact with
    private GameObject interactedObject;

    //camera realted
    private Vector2 cameraDisplacement;
    private float pitch = 0f;
    private float yaw = 0f;
    private float targetSpeed = 0f;

    //state machine related
    WaterHorizontal waterHorizontal;
    WaterVertical waterVertical;
    InteractType interactType = InteractType.None;

    void OnEnable(){
	UserInput.moveEvent += OnMove;
	UserInput.lookEvent += OnLook;
	UserInput.moveCanceledEvent += OnMoveStop;
	UserInput.upEvent += OnUp;
	UserInput.downEvent += OnDown;
	UserInput.upCanceledEvent += OnUpStop;
	UserInput.downCanceledEvent += OnDownStop;
	UserInput.interactEvent += OnInteract;
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
	handleMovement();
	handleCamera(cameraDisplacement);
	handleRaycast();
    }

    private void handleMovement(){
	//pass vertical and horizontal directions to determine player states
	targetSpeed = updateStateMachines(horizontalDirection,verticalDirection);

	Vector3 targetDirection = transform.rotation * horizontalDirection;
	targetDirection += verticalDirection;
	Vector3 targetVelocity = targetDirection.normalized * targetSpeed;
	currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

	controller.Move(currentVelocity * Time.deltaTime);
    }

    private void handleCamera(Vector2 mouseDelta){
	pitch -= mouseDelta.y * lookSensitivity;
	pitch = Math.Clamp(pitch,-maxCameraRotation, maxCameraRotation);
	yaw += mouseDelta.x * lookSensitivity;
	transform.rotation = Quaternion.Euler(pitch,yaw,0);
    }

    private void handleRaycast(){
	Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
	if (Physics.Raycast(ray, out RaycastHit hit, interactRange,layerMask)){
	    hitInteractable = true;
	    interactedObject = hit.collider.gameObject;
	    if(interactedObject.layer == 6){
		interactType = InteractType.Collectible;
	    }
	    else if(interactedObject.layer == 7){
		interactType = InteractType.Placeable;
	    }
	    
	}
	else{
	    hitInteractable = false;
	    interactedObject = null;
	    interactType = InteractType.None;
	}
    }

    private float updateStateMachines(Vector3 horizontal, Vector3 vertical){
	if(horizontal != Vector3.zero){
	    waterHorizontal = WaterHorizontal.Swim;
	    animator.speed = 2f;
	}
	else{
	    waterHorizontal = WaterHorizontal.Idle;
	    animator.speed = 0.5f;
	}

	if(vertical.y > 0f){
	    waterVertical = WaterVertical.Ascend;
	}
    	
	else if(vertical.y < 0f){
	    waterVertical = WaterVertical.Descend;
	}
	
	else{
	    waterVertical = WaterVertical.Idle;
	}

	if(waterVertical == WaterVertical.Idle && waterHorizontal == WaterHorizontal.Idle){
	    return 0f;
	}

	else{
	    return maxSpeed;
	}




	
    }

    //reading and storing the values of the input events
    private void OnMove(Vector2 direction){
	    horizontalDirection = new Vector3(direction.x,0,direction.y);
	    targetSpeed = maxSpeed;
    }

    private void OnLook(Vector2 displacement){
	cameraDisplacement = displacement; 
    }


    private void OnMoveStop(Vector2 directionZero){
	horizontalDirection = directionZero;
    }

    private void OnUp(){
	verticalDirection = verticalDirection + new Vector3(0f,1f,0f);
    }

    private void OnDown(){
	verticalDirection = verticalDirection - new Vector3(0f,1f,0f);
    }

    private void OnDownStop(){
	verticalDirection = verticalDirection + new Vector3(0f,1f,0f);
    }

    private void OnUpStop(){
	verticalDirection = verticalDirection - new Vector3(0f,1f,0f);
    }
    
    private void OnInteract(){
	if(InteractSystem.instance == null){
	    Debug.Log("Missing Interact Manager");
	    return;
	}
	if(hitInteractable && interactType == InteractType.Collectible && InteractSystem.instance.listContainsCollectible(interactedObject)){
	    InteractSystem.instance.Collect(interactedObject);
	}		
	else if(hitInteractable && interactType == InteractType.Placeable ){
	    InteractSystem.instance.Place(interactedObject);
	}
    }


}
