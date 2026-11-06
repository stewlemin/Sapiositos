using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

//anything

public class PlayerController : MonoBehaviour
{
    //Inspector exposed variables 
    [SerializeField] Camera mainCamera;
    [SerializeField] UserInput userInput;
    //change this to a different script later on
    [SerializeField] Animator animator;
    //temp prototype variables move later intro scriptable objects???
    //maybe change location later
    [SerializeField] float lookSensitivity;
    [SerializeField] float maxCameraRotation;
    [SerializeField] float maxSpeed = 3f;
    [SerializeField] float acceleration = 2;
    [SerializeField] float interactRange = 2.0f;

    [SerializeField] float tiltAngle = 5.0f;
    [SerializeField] float tiltSpeed = 0.5f;


    public bool isPressingKey => horizontalDirection != Vector3.zero || verticalDirection != Vector3.zero; //for patroller



    //Movement realted states undewater
    enum WaterHorizontal{
        Idle,
	Swim,
    }

    enum WaterVertical{
	Idle,
	Ascend,
	Descend
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
    private bool isUpPressed = false;
    private bool isDownPressed = false;

    //interact related
    private bool hitInteractable = false;
    private int layerMask = 192;//6th and 7th bit are 1 coresponding to the layers that you can interact with
    private GameObject interactedObject;

    //camera realted
    private float pitch = 0f;
    private float yaw = 0f;
    private float targetSpeed = 0f;
    private float currentRoll = 0f;

    //state machine related
    WaterHorizontal waterHorizontal;
    WaterVertical waterVertical;
    InteractType interactType = InteractType.None;
    string lastTriggerSent;

    void OnEnable(){
	userInput.EnableActions();
	userInput.upEvent += OnUp;
	userInput.downEvent += OnDown;
	userInput.upCanceledEvent += OnUpStop;
	userInput.downCanceledEvent += OnDownStop;
	userInput.interactEvent += OnInteract;
    }

    void OnDisable(){
	userInput.upEvent -= OnUp;
	userInput.downEvent -= OnDown;
	userInput.upCanceledEvent -= OnUpStop;
	userInput.downCanceledEvent -= OnDownStop;
	userInput.interactEvent -= OnInteract;
    }

    void Awake()
    {
	controller = gameObject.GetComponent<CharacterController>();
    }

    void Start()
    {
	Cursor.visible = false;
	Cursor.lockState = CursorLockMode.Locked;
	animator.Play("Rig|Idle_Swim",0);
    }

    void Update()
    {
	//browsers only allow pointer lock after a click, so locking in Start() does nothing on WebGL
	if(Cursor.lockState != CursorLockMode.Locked && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame){
	    Cursor.lockState = CursorLockMode.Locked;
	    Cursor.visible = false;
	}

	//polled like look, so movement doesn't depend on performed/canceled events arriving
	Vector2 moveInput = userInput.moveAction.ReadValue<Vector2>();
	horizontalDirection = new Vector3(moveInput.x, 0, moveInput.y);

	handleMovement();
	//polled instead of event driven, pointer delta is per frame and the look event isn't guaranteed to reset it
	handleCamera(userInput.lookAction.ReadValue<Vector2>());
	handleRaycast();
	handleAnimations();
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

	float targetRoll = 0f;
	if(horizontalDirection.x < -0.1f) targetRoll = tiltAngle;
	else if(horizontalDirection.x > 0.1f) targetRoll = -tiltAngle; 
	
	//camera roll on sway movement
	currentRoll = Mathf.Lerp(currentRoll, targetRoll, tiltSpeed * Time.deltaTime);

	transform.rotation = Quaternion.Euler(pitch,yaw,currentRoll);

	//underwater viewbob
	if(mainCamera != null){
	    
	}
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


    private void updateVertical(){
	float y = 0;
	if(isUpPressed){
	    y += 1;
	}
	if(isDownPressed){
	    y -= 1;
	}
	if(!isUpPressed && !isDownPressed){
	    y = 0;
	}
	verticalDirection = new Vector3(0.0f , y, 0.0f);
    }



    private float updateStateMachines(Vector3 horizontal, Vector3 vertical){
	if(horizontal != Vector3.zero){
	    waterHorizontal = WaterHorizontal.Swim;
	}
	else{
	    waterHorizontal = WaterHorizontal.Idle;
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

    private void handleAnimations(){
	if (waterHorizontal == WaterHorizontal.Idle)
        {
            animator.SetInteger("MotionState", 0);
            animator.speed = 1f;
        }
        else 
        {
            if (horizontalDirection.x < -0.1f)
            {
                animator.SetInteger("MotionState", 1); 
                animator.speed = 1f;
            }
            else if (horizontalDirection.x > 0.1f)
            {
                animator.SetInteger("MotionState", 2); 
                animator.speed = 1f;
            }
            else
            {
                animator.SetInteger("MotionState", 0);
                animator.speed = 2f;
            }
        }
    }


    //reading and storing the values of the input events
    private void OnUp(){
	isUpPressed = true;
	updateVertical();
    }

    private void OnUpStop(){
	isUpPressed = false;
	updateVertical();
    }

    private void OnDown(){
	isDownPressed = true;
	updateVertical();
    }

    private void OnDownStop(){
	isDownPressed = false;
	updateVertical();
    }

    private void OnInteract(){
	if(hitInteractable){
	    animator.SetTrigger("Collect");
	}
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
