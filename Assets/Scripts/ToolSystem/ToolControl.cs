using UnityEngine;
using UnityEngine.InputSystem;

public class ToolControl : MonoBehaviour
{
    public Detector detector;
    public Flashlight flash;

    public void EquipFlashlight()
    {
        if (flash != null) flash.Equip();
        if (detector != null) detector.Unequip(); 
    }
    
    public void EquipDetector()
    {
        if (detector != null) detector.Equip();
        if (flash != null) flash.Unequip(); 
    }

    public InputAction equipFlashlightAction = new InputAction("EquipFlashlight", InputActionType.Button, "<Keyboard>/1");
    public InputAction equipDetectorAction = new InputAction("EquipDetector", InputActionType.Button, "<Keyboard>/2");
    
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

    void Start()
    {
        EquipFlashlight(); 
    }

    void Update()
    {
        if (equipFlashlightAction.WasPressedThisFrame())
        {
            EquipFlashlight(); 
        }

        if (equipDetectorAction.WasPressedThisFrame())
        {
            EquipDetector(); 
        }
    }
}