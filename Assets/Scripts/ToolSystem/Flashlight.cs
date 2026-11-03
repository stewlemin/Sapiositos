using UnityEngine;
using UnityEngine.InputSystem;



public class Flashlight : MonoBehaviour
{
    //flashlight
    [SerializeField] private Light spotlight;
    [SerializeField] private GameObject flashobject;
    [SerializeField] private InputAction toggleAction = new InputAction("ToggleFlashlight", InputActionType.Button, "<Mouse>/rightButton");
    private bool isOn = true;
    private bool isEquipped; 
    
    //battery
    public float fullbattery = 10f; //maximum battery capacity
    [Range(0f, 1f)]
    public float lowbattery = 0.5f; //low = when battery has 50% of battery 
    public float currentbattery; //tracks current battery while playing
    public float drainpersecond = 0.05f;//amount of battery that drains/second when on
    public float timetofullycharge = 60f; // total time to charge fully when not equipped
    public float criticaltime = 5f;
    //intensity 
    public float lowintensity = 0.7f;//intensity of spotlight when low battery
    public float criticalintensity = 0.4f;//intensity of spotlight when critical battery
    private float fullintensity;//start intensity

    //flicker
    private bool isflickering;
    private float flickertimer;//countdown flicker intervals
    public float lowflickerinterval = 0.2f;//flciker interval when battery low
    public float criticalflickerinterval = 0.08f;//flicker interval when battery critical
    

    //flashlight has 4 states: off, high battery (no flicker), low battery(flickers a bit), critical battery(>flicker, <intensity)
    private enum flashstate {off, high, low, critical}
    private flashstate state = flashstate.high;//current state 

    void Awake()
    {
        currentbattery = fullbattery;
        if(spotlight != null)
        {
            fullintensity = spotlight.intensity;
        }
    }

    void Start()
    {
        Changestate(flashstate.high);
    }

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
        if (flashobject != null)
        {
            flashobject.SetActive(true);    
        } 
    }
    
    public void Unequip()
    {
        isEquipped = false;  
        if (flashobject != null)
        {
            flashobject.SetActive(false);   
        } 
    }
    
    void Update()
    {
        if(spotlight == null)
        return;
        //can toggle only when equipped
        if (isEquipped && toggleAction.WasPressedThisFrame())
        {
            if (isOn)
            {
                isOn = false;//turn off flashlight if it is equipped and right click mouse was pressed
            }
            else if (currentbattery >= 0f)
            {
                isOn = true;//if battery had enough charge, then turn it on
            }
        }

        bool lightIsOn = isEquipped && isOn; //lightens only if equipped and flashlight works
        if (lightIsOn)
        {
            currentbattery = currentbattery - drainpersecond * Time.deltaTime;//drain battery life a bit every frame
            if (currentbattery <= 0) 
            {
                currentbattery = 0;        
                isOn = false;       
                lightIsOn = false;  
            }
        }
        //automatically calculate critical threshold based on criticaltime(how many seconds to last)
        float criticalbatteryamount = drainpersecond * criticaltime;

        //determine which state is flashlight currently on
        flashstate wanted; 

        if(!lightIsOn) 
        wanted = flashstate.off;//set off when light not on
        else
        //check if battery reached critical
        if(currentbattery<= criticalbatteryamount)
        wanted = flashstate.critical;
        else
        //check if battery below low battery
        if(currentbattery<= fullbattery * lowbattery)
        wanted = flashstate.low;
        else
        wanted = flashstate.high;

        //change state if it is needed
        if (wanted != state)
        Changestate(wanted);

        switch (state)
        {   
            //in low state run low flickering
            case flashstate.low:
            Lowflicker();
            break;

            //in critical state run criticla fickering
            case flashstate.critical:
            Criticalflicker();
            break;
        }

        spotlight.enabled = lightIsOn;

        //recharge flashlight when not equipped and below full capacity
        if (!isEquipped && currentbattery < fullbattery)
        {   
            currentbattery = Mathf.Min(fullbattery, currentbattery + (fullbattery/timetofullycharge)*Time.deltaTime);
        }
    }

    //update flicker timer and intensities when state changes
    private void Changestate(flashstate newstate)
    {
        state = newstate;
        isflickering = false;
        flickertimer = 0f;

        if (newstate == flashstate.high)
        {
            spotlight.intensity = fullintensity;
        }
        else if (newstate == flashstate.low)
        {
            spotlight.intensity = fullintensity *lowintensity;
        }
        else if(newstate == flashstate.critical)
        {
            spotlight.intensity = fullintensity * criticalintensity;
        }
    
    }

    private void Lowflicker()
    {
        float instensitystate = fullintensity *lowintensity;//calculated intensity for low state
        flickertimer = flickertimer - Time.deltaTime;

        if(flickertimer <= 0f)
        {
           isflickering = !isflickering;
           if (isflickering)
           {
            spotlight.intensity = instensitystate *0.5f;//when flickers lower intensity
           }
           else
           {
            spotlight.intensity = instensitystate;
           }

        flickertimer = lowflickerinterval;
        }
    }

    private void Criticalflicker()
    {
        float instensitystate = fullintensity *criticalintensity;
        flickertimer = flickertimer - Time.deltaTime;

        if(flickertimer <= 0f)
        {
            isflickering = !isflickering;
            
            if (isflickering)
            {
                spotlight.intensity = instensitystate *0.1f; 
            }
            else
            {
                spotlight.intensity = instensitystate;
            }

            flickertimer = criticalflickerinterval;
        }     
    }
}

   