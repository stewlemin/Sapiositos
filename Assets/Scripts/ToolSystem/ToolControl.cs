using UnityEngine;
using UnityEngine.InputSystem;

public class ToolControl : MonoBehaviour
{
    public Detector detector;
    public Flashlight flash;

    private enum ToolState {None , Flashlight, Detector}
    private ToolState toolState;
	

    public InputAction equipFlashlightAction = new InputAction("EquipFlashlight", InputActionType.Button, "<Keyboard>/1");
    public InputAction equipDetectorAction = new InputAction("EquipDetector", InputActionType.Button, "<Keyboard>/2");

    //overhaul the tool system so it works with events. Make disabled states for both of the tools instead of using .SetActive() since that messes up OnEnable and OnDisable
    
    void OnEnable() 
    {
       equipFlashlightAction.Enable();
       equipDetectorAction.Enable();
    }
 
    void OnDisable() 
    {
        equipFlashlightAction.Disable();
        equipDetectorAction.Disable();
    }

    void Update()
    {
	Debug.Log(toolState);
	if(flash == null || detector == null){
	    Debug.Log("tools references missing");
	    return;
	}
        if (equipFlashlightAction.WasPressedThisFrame())
        {
	    if(toolState != ToolState.Flashlight){
		detector.PowerOff();
		flash.PowerOn();
		toolState = ToolState.Flashlight;
	    }
	    else{
		flash.PowerOff();
		toolState = ToolState.None;
	    }
        }

	else if (equipDetectorAction.WasPressedThisFrame())
        {
	    if(toolState != ToolState.Detector){
		flash.PowerOff();
		detector.PowerOn();
		toolState = ToolState.Detector;
	    }
	    else{
		detector.PowerOff();
		toolState = ToolState.None;
	    }
        }
    }

	
}
