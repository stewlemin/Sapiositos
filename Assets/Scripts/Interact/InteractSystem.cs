using UnityEngine;
using System.Collections.Generic;

public class InteractSystem : MonoBehaviour
{
    public static InteractSystem instance {get; private set;}

    //list containining references to objects, exposed in inspector
    [SerializeField] private List<GameObject> collectibles = new List<GameObject>();
    [SerializeField] SoundVariations collectSounds;
    [SerializeField] SoundVariations placeSounds;
    
    //number of collected objects
    private int artefactsOnPlayer = 0;
    private int totalCollected = 0; //added this to fix a bug where Collect() sends the wrong number to the stalker, 
									//because artefactsOnPlayer decreases, when you place an artefact it goes down
									//for example if collect #1 and place it and collect the next one, the count is till at 1
									//the schedule entry for artefact 1 fires again and entry 2 never does
    //event for detector
    public delegate void PickupEvent();
    public event PickupEvent OnArtefactPickUp;

    
    void Awake(){
	if(instance != null && instance != this){
	    Destroy(this);
	}
	else{
	    InteractSystem.instance = this;
	}
    }
    
    //total count of collectible objects
    public int getCollectibleCount(){
	return collectibles.Count;
    }

    public List<GameObject> getCollectibleList(){
	return new List<GameObject>(collectibles);
    }

    //wether collectible list contains item, collectible list is private
    public bool listContainsCollectible (GameObject collectible){
	return collectibles.Contains(collectible);
    }


    //destroys the object and incerements the number of collected objects
    public void Collect(GameObject objectToRemove){
	collectibles.Remove(objectToRemove);
	if(InteractAudio.instance != null){
	    if(collectSounds != null){
		InteractAudio.instance.playGlobalAudio(collectSounds?.getSound());//objectToRemove.transform.position
	    }
	    else{
		Debug.Log("CollectSounds are missing from InteractManager");
	    }
	}
	else{
	    Debug.Log("InteractAudio missing from scene");
	}
	Destroy(objectToRemove);
	OnArtefactPickUp?.Invoke();
	artefactsOnPlayer += 1;
	totalCollected += 1;
	MonsterEventChannel.BroadcastArtefactCollected(totalCollected);
	Debug.Log(artefactsOnPlayer);
    }

    public void Place(GameObject objectToTryPlace){
	if(objectToTryPlace.TryGetComponent<ArtefactSlot>(out ArtefactSlot artefactSlotScript)){
	    artefactSlotScript.TryPlace(artefactsOnPlayer);
	    	
	}	
	else{
	    Debug.Log("Placeable object does not contain artefactSlot component");
	}
    }

    public void decrementCollectedObjects(Vector3 position){
	artefactsOnPlayer -= 1;
	//added this guard for a null check vvv
	if (InteractAudio.instance != null)
		{
			InteractAudio.instance.playGlobalAudio(placeSounds?.getSound());
		}
		Debug.Log(artefactsOnPlayer);
    }


}
