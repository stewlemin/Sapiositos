using UnityEngine;
using UnityEngine.InputSystem;

public class ToolControl : MonoBehaviour
{
    public Detector detector;
    public Flashlight flash;

    private enum ToolState {None , Flashlight, Detector}
    private ToolState toolState;


    public InputAction cycleTool = new InputAction("CycleTool", InputActionType.Button, "<Keyboard>/q");

    //overhaul the tool system so it works with events. Make disabled states for both of the tools instead of using .SetActive() since that messes up OnEnable and OnDisable

    void OnEnable() 
    {
       cycleTool.Enable();
    }
 
    void OnDisable() 
    {
        cycleTool.Disable();
    }

    void Update()
    {
        if(flash == null || detector == null)
        {
            Debug.Log("tools references missing");
            return;
        }

        if (cycleTool.WasPressedThisFrame())
        {
            int countState = System.Enum.GetValues(typeof(ToolState)).Length; //next state in enum, wraps back to none after the last one
            int current = (int)toolState;
            int next = (current + 1) % countState;
            ToolState nextTool = (ToolState)next;
            EquipTool(nextTool);
        }

    }

    //turn off both tools then power the one we want, none for unequip
    private void EquipTool(ToolState newState)
    {
        flash.PowerOff();
        detector.PowerOff();

        switch (newState)
        {
            case ToolState.Flashlight: flash.PowerOn(); break;
            case ToolState.Detector: detector.PowerOn(); break;
        }
        toolState = newState;
    }


}