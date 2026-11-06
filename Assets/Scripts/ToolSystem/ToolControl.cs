using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ToolControl : MonoBehaviour
{
    public Detector detector;
    public Flashlight flash;

    private enum ToolState {None , Flashlight, Detector}
    private ToolState toolState;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject flashlightModel;
    [SerializeField] private GameObject detectorModel;
    [SerializeField] private float equipDelay = 0.2f;
    [SerializeField] private float unequipDelay = 0.2f;


    public InputAction cycleTool = new InputAction("CycleTool", InputActionType.Button, "<Keyboard>/q");
    private Coroutine flashlightCoroutine;
    private Coroutine detectorCoroutine;

    //overhaul the tool system so it works with events. Make disabled states for both of the tools instead of using .SetActive() since that messes up OnEnable and OnDisable

    void OnEnable() 
    {
       cycleTool.Enable();
    }
 
    void OnDisable() 
    {
        cycleTool.Disable();
    }

    void Start(){
	flashlightModel.SetActive(false);
	detectorModel.SetActive(false);
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
            case ToolState.Detector: StartCoroutine(detectorPowerOn(detector)); break;
        }
	PlayToolAnimation(toolState, newState);
        toolState = newState;
    }
    

    private void PlayToolAnimation(ToolState oldState, ToolState newState)
    {
        if (playerAnimator != null && flashlightModel != null && detectorModel != null)
        {
            if (oldState != ToolState.None && newState == ToolState.None)
            {
                if (oldState == ToolState.Flashlight) UnequipFlashlight();
                if (oldState == ToolState.Detector) UnequipDetector();
            }
            else if (oldState == ToolState.None && newState != ToolState.None)
            {
                if (newState == ToolState.Flashlight) EquipFlashlight();
                if (newState == ToolState.Detector) EquipDetector();
            }
            else if (oldState != ToolState.None && newState != ToolState.None && oldState != newState)
            {
                if (oldState == ToolState.Flashlight) UnequipFlashlight();
                if (oldState == ToolState.Detector) UnequipDetector();

                if (newState == ToolState.Flashlight) EquipFlashlight();
                if (newState == ToolState.Detector) EquipDetector();
            }
        }
        else
        {
            Debug.Log("Missing reference to either flashlightModel, detectorModel or animator");
        }
    }
    
    private void EquipFlashlight()
    {
        playerAnimator.SetTrigger("EquipFlashlight");
        if (flashlightCoroutine != null) StopCoroutine(flashlightCoroutine);
       flashlightCoroutine = StartCoroutine(ToggleWithDelay(flashlightModel, true, equipDelay));
    }

    private void UnequipFlashlight()
    {
        playerAnimator.SetTrigger("UnequipFlashlight");
        if (flashlightCoroutine != null) StopCoroutine(flashlightCoroutine);
        flashlightCoroutine = StartCoroutine(ToggleWithDelay(flashlightModel, false, unequipDelay));
    }

    private void EquipDetector()
    {
        playerAnimator.SetTrigger("EquipDetector");
        if (detectorCoroutine != null) StopCoroutine(detectorCoroutine);
        detectorCoroutine = StartCoroutine(ToggleWithDelay(detectorModel, true, equipDelay));
    }

    private void UnequipDetector()
    {
        playerAnimator.SetTrigger("UnequipDetector");
        if (detectorCoroutine != null) StopCoroutine(detectorCoroutine);
        detectorCoroutine = StartCoroutine(ToggleWithDelay(detectorModel, false, unequipDelay));
    }

    private IEnumerator ToggleWithDelay(GameObject targetObject, bool activate, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (targetObject != null)
        {
            targetObject.SetActive(activate);
        }
    }

    private IEnumerator detectorPowerOn(Detector detector){
	yield return new WaitForSeconds(0.6f);
	detector.PowerOn();
    }

}
