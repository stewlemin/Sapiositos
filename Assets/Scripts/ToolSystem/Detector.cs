using UnityEngine;

public class Detector : MonoBehaviour
{
    public AudioClip beepclip;         
    public float maxRange = 50f;       
    public float slowBeep = 1.5f;    
    public float fastBeep = 0.1f;   
    public float lowPitch = 0.8f;    
    public float highPitch = 1.5f;   
    public float panStrength = 1f;  
    public float closeness; 
    public float pan;        
    ArtifactsDetector scanner;
    AudioSource audiosource;
    float beeptimer; //countdown between beeps

    void Awake()
    { 
        scanner = GetComponent<ArtifactsDetector>();
        audiosource = GetComponent<AudioSource>();
        if (scanner != null) scanner.maxrange = maxRange;
        if (audiosource != null) audiosource.spatialBlend = 0;//detector sounds beyond walls
    }

    public void Equip()
    {
        gameObject.SetActive(true); //turn on ehwn equipped
    }
    
    public void Unequip()
    {
        gameObject.SetActive(false); //turn off when unequipped
    }
    void Update()
    {
        if (scanner == null) return;

        Artifact nearest = scanner.FindNearest(); //search closest collectible
        if (nearest == null) //nothing found
        {
            closeness = 0; //nothing near
            pan = 0;
            return;
        }

        closeness = 1 - scanner.distance / maxRange; //transform distance

        Vector3 local = transform.InverseTransformPoint(nearest.transform.position); //get collectible position

        pan = local.x / Mathf.Max(local.magnitude , 0.01f); // divide sound distance based on x 
        float waitTime = Mathf.Lerp(slowBeep, fastBeep, closeness);//smooths detector fast and slow states between them

        beeptimer -= Time.deltaTime;

        if (beeptimer <= 0) //if timer =0
        {
            if (audiosource != null && beepclip != null)
            {
                audiosource.panStereo = pan * panStrength; //return sound to left or right ear
                audiosource.pitch = Mathf.Lerp(lowPitch, highPitch, closeness); //change pitch based on distance
                audiosource.PlayOneShot(beepclip); //beeps once
            }
            beeptimer = waitTime; 
        }
    }
}
