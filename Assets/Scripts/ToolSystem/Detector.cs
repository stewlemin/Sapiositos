using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    //inspector exposed
    [SerializeField] private float pollingInterval = 1.5f;
    [SerializeField] private float distanceScaling = 1.0f;
    [SerializeField] private float timeBetweenBeeps = 2.0f; //seconds
    [SerializeField] private float maxDistanceScaling = 1;
    [SerializeField] private AudioSource beeperSource;
    [SerializeField] private AudioClip farBeep;
    [SerializeField] private AudioClip midBeep;
    [SerializeField] private AudioClip closeBeep;
    [SerializeField] private AudioClip foundBeep;
    [SerializeField] private AudioClip rechargeSound;

    //tracking related variables
    private List<GameObject> collectiblesList;
    private GameObject targetObject;

    //detector state machie
    private enum DetectorState{Disabled ,None, Tracking, Recharging}

    //variable intializations
    private DetectorState detectorState = DetectorState.Disabled;
    float elapsedTime = 0.0f;
    float beepTimer = 0.0f;
    float unit = 10f;

    public void Start(){
        if(InteractSystem.instance != null){
	    collectiblesList = InteractSystem.instance.getCollectibleList();
        }
	if(beeperSource != null){
	    beeperSource.loop = true;
	}

    }

    void OnEnable(){
	Debug.Log(InteractSystem.instance);
        if(InteractSystem.instance != null){
            InteractSystem.instance.OnArtefactPickUp += RefreshList;
        }
        else{
            Debug.Log("Missing interact system");
        }
    }

    void OnDisable(){

        if(InteractSystem.instance != null){
            InteractSystem.instance.OnArtefactPickUp -= RefreshList;
        }
    }

    private void RefreshList(){
	Debug.Log("refresh happned");
	if(detectorState != DetectorState.Disabled){
	    beeperSource.clip = rechargeSound;
	    beeperSource.Play();
	    detectorState = DetectorState.Recharging;
	    StartCoroutine(Recharge());
	}	
        collectiblesList = InteractSystem.instance.getCollectibleList();
    }

    IEnumerator Recharge(){
        yield return new WaitForSeconds(pollingInterval);
        elapsedTime = 0;
	targetObject = null;
	if(detectorState != DetectorState.Disabled){
	    detectorState = PollObjects(collectiblesList);
	}
    }


    private DetectorState PollObjects(List<GameObject> collectibles){
	if(collectibles == null){
	    Debug.Log("collectibles is Empty, at point in time");
	    return DetectorState.None;
	}
        if (collectibles.Count == 0){
            targetObject = null;
            return DetectorState.None;
        }
        else{
            targetObject = FindClosest(collectibles);
            return DetectorState.Tracking;
        }
    }

    private GameObject FindClosest(List<GameObject> collectibles){
        float min = Mathf.Infinity;
        GameObject minObject = null;
        for(int i =0 ; i < collectibles.Count; i++){
	    if(collectibles[i] == null){
		continue;
	    }
            Vector3 toObject = collectibles[i].transform.position - transform.position;
            float distance = toObject.magnitude;
            if(distance < min){
                min = distance;
                minObject = collectibles[i];
            }
        }
        return minObject;
    }

    void Update(){
	if(InteractSystem.instance == null)return;
	if(detectorState == DetectorState.Disabled){
	    beeperSource.clip = null;
	    return;	
	}
	// if(detectorState == DetectorState.None){
	//     if(beeperSource != null && beeperSource != rechargeSound){
	// 	beeperSource.clip = rechargeSound;
	// 	beeperSource.loop = true;
	// 	beeperSource.Play();
	//     }
	// }
        if(detectorState == DetectorState.None && InteractSystem.instance.getCollectibleCount() == 0 || detectorState == DetectorState.Recharging ) {
            return;
        }

        if(elapsedTime >= pollingInterval){
            detectorState = PollObjects(collectiblesList);
            elapsedTime = 0;
        }
	
        elapsedTime += Time.deltaTime;

	if(targetObject != null){
	    float distance = CalculateDistance(targetObject);
	    if(distance > 0f && beeperSource != null){
		if(distance < 0.05f * unit * distanceScaling){//less than 5%
		    if(beeperSource.clip != foundBeep){
			beeperSource.clip = foundBeep;
			beeperSource.loop = true;
			if(!beeperSource.isPlaying)
			    beeperSource.Play();
			}
		    }
		else if(distance < 0.15f * unit * distanceScaling){//less than 15%
		    if(beeperSource.clip != closeBeep){
			beeperSource.clip = closeBeep;
			beeperSource.loop = true;
			beeperSource.Play();
		    }
		}
		else if(distance < 0.30f * unit * distanceScaling){//less than 30%
		    if(beeperSource.clip != midBeep){
			beeperSource.clip = midBeep;
			beeperSource.loop = true;
			beeperSource.Play();
		   }
		}

		else{
			beeperSource.loop = false;
			beeperSource.clip = null;
			float maxFarDistance = maxDistanceScaling * unit * distanceScaling;
			float currentInterval = Mathf.Clamp((distance/maxFarDistance),0f,1f) * timeBetweenBeeps;

			beepTimer += Time.deltaTime;
			if(beepTimer > currentInterval){
			    beeperSource.PlayOneShot(farBeep);
			    beepTimer = 0f;
			}
		    }
		}
	    }
	}
	

    public float CalculateDistance(GameObject target){
	return (target.transform.position - transform.position).magnitude;
    }

    public void PowerOn(){
	RefreshList();
	detectorState = PollObjects(collectiblesList);
	Debug.Log(detectorState);
    }

    public void PowerOff(){
	detectorState = DetectorState.Disabled;
    }
}

