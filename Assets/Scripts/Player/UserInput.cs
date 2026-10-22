using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UserInput", menuName = "Scriptable Objects/UserInput")]
public class UserInput : ScriptableObject
{
    public InputActionAsset inputAsset;

    public InputAction moveAction;
    public InputAction lookAction;
    public InputAction interactAction;
    public InputAction upAction;
    public InputAction downAction;
    public InputAction sprintAction;

    //assigning events for each of the input actions, this approach is preffered rather then polling in Update()
    public delegate void MoveEvent(Vector2 direction);
    public static event MoveEvent moveEvent;
    public static event MoveEvent moveCanceledEvent;

    public delegate void LookEvent(Vector2 direction);
    public static event LookEvent lookEvent;

    public delegate void InteractEvent();
    public static event InteractEvent interactEvent;

    public delegate void UpEvent();
    public static event UpEvent upEvent;

    public delegate void DownEvent();
    public static event DownEvent downEvent;

    public delegate void SprintEvent();
    public static event SprintEvent sprintEvent;
    

    void OnEnable(){
	//assigning input actions from .inputactions file
	moveAction = inputAsset.FindAction("Move");
	lookAction = inputAsset.FindAction("Look");
	interactAction = inputAsset.FindAction("Interact");
	upAction = inputAsset.FindAction("Up");
	downAction = inputAsset.FindAction("Down");
	sprintAction = inputAsset.FindAction("Sprint");
	
	//assigning each input event a function
	moveAction.started += OnMove;
	moveAction.performed += OnMove;
	moveAction.canceled += OnMove;

	lookAction.started += OnLook;
	lookAction.performed += OnLook;
	lookAction.canceled += OnLook;

	interactAction.started += OnInteract;
	interactAction.performed += OnInteract;
	interactAction.canceled += OnInteract;

	upAction.started += OnUp;
	upAction.performed += OnUp;
	upAction.canceled += OnUp;

	downAction.started += OnDown;
	downAction.performed += OnDown;
	downAction.canceled += OnDown;

	sprintAction.started += OnSprint;
	sprintAction.started += OnSprint;
	sprintAction.canceled += OnSprint;

	//enabling input actions
	moveAction.Enable();
	lookAction.Enable();
	interactAction.Enable();
	upAction.Enable();
	downAction.Enable();
	sprintAction.Enable();
    }

    void OnDisable(){
	//unsubscribing from InputAction events 
	moveAction.started -= OnMove;
	moveAction.performed -= OnMove;
	moveAction.canceled -= OnMove;

	lookAction.started -= OnLook;
	lookAction.performed -= OnLook;
	lookAction.canceled -= OnLook;

	interactAction.started -= OnInteract;
	interactAction.performed -= OnInteract;
	interactAction.canceled -= OnInteract;

	upAction.started -= OnUp;
	upAction.performed -= OnUp;
	upAction.canceled -= OnUp;

	downAction.started -= OnDown;
	downAction.performed -= OnDown;
	downAction.canceled -= OnDown;

	sprintAction.started -= OnSprint;
	sprintAction.started -= OnSprint;
	sprintAction.canceled -= OnSprint;

	moveAction.Disable();
	lookAction.Disable();
	interactAction.Disable();
	upAction.Disable();
	downAction.Disable();
	sprintAction.Disable();
	

    }
    
    //functions that the Actions call upon user interaction
    void OnMove(InputAction.CallbackContext context){
	if(moveEvent != null && context.performed){
	    moveEvent.Invoke(context.ReadValue<Vector2>());
	}
	else if(moveCanceledEvent != null && context.canceled){
	    moveCanceledEvent.Invoke(context.ReadValue<Vector2>());
	}
    }

    void OnLook(InputAction.CallbackContext context){
	lookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    void OnInteract(InputAction.CallbackContext context){}

    void OnUp(InputAction.CallbackContext context){}

    void OnDown(InputAction.CallbackContext context){}

    void OnSprint(InputAction.CallbackContext context){}




    
}
