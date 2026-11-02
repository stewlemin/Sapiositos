using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    public Light spotlight;
    public GameObject flashprop;
    public InputAction toggleAction = new InputAction("ToggleFlashlight", InputActionType.Button, "<Mouse>/rightButton");
    public float drainpersecond = 0.05f;
    public float battery = 1f; 
    bool isOn = true;
    bool isEquipped; 

    void OnEnable()  
    {
        toggleAction.Enable();  
    }
 
    void OnDisable()  
    {
        toggleAction.Disable();  
    }

    public void Equip()
    {
        isEquipped = true;
        if (flashprop != null) flashprop.SetActive(true); 
    }
    
    public void Unequip()
    {
        isEquipped = false;  
        if (flashprop != null) flashprop.SetActive(false); 
    }

    void Update()
    {
        if (isEquipped && toggleAction.WasPressedThisFrame())
        {
            if (isOn)
            {
                isOn = false;//turn off flashlight if it is equipped and right click mouse was pressed
            }
            else if (battery >= 0.1f)
            {
                isOn = true;//if battery has enough charge, then turn it on
            }
        }

        bool lightIsOn = isEquipped && isOn; // Lightens only if equipped and flashlight works
        if (lightIsOn)
        {
            battery -= drainpersecond * Time.deltaTime;//drain battery life a bit every frame
            if (battery <= 0) 
            {
                battery = 0;        
                isOn = false;       
                lightIsOn = false;  
            }
        }
        
        if (spotlight != null) spotlight.enabled = lightIsOn;
        
    }
}