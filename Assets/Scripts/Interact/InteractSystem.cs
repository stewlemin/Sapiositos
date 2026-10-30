using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class InteractSystem : MonoBehaviour
{
    public static InteractSystem instance {get; private set;}

    //list containining references to objects, exposed in inspector
    [SerializeField] private List<GameObject> collectibles = new List<GameObject>();
    [SerializeField] SoundVariations collectSounds;
    [SerializeField] SoundVariations placeSounds;
    
    //number of collected objects
    private int collectedObjects = 0;

    //use "InteractSystem.instance.OnArtefactPickup += YourHandled" to subscribe to this event
    public delegate void PickUpEvent(int artefactNumber);
    public static event PickUpEvent OnArtefactPickup;
    
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

    //wether collectible list contains item, collectible list is private
    public bool listContainsCollectible (GameObject collectible){
	return collectibles.Contains(collectible);
    }


    //destroys the object and incerements the number of collected objects
    public void Collect(GameObject objectToRemove){
	collectibles.Remove(objectToRemove);
	Debug.Log(collectSounds.name);
	Debug.Log(InteractAudio.instance);
	InteractAudio.instance.playGlobalAudio(collectSounds?.getSound());//objectToRemove.transform.position
	Destroy(objectToRemove);
	collectedObjects += 1;
    MonsterEventChannel.BroadcastArtefactCollected(collectedObjects);
	Debug.Log(collectedObjects);
    }
    

    public void Place(GameObject objectToTryPlace){
	if(objectToTryPlace.TryGetComponent<ArtefactSlot>(out ArtefactSlot artefactSlotScript)){
	    artefactSlotScript.TryPlace(collectedObjects);
	    	
	}	
	else{
	    Debug.Log("Placeable object does not contain artefactSlot component");
	}
    }

    public void decrementCollectedObjects(Vector3 position){
	collectedObjects -= 1;
	InteractAudio.instance.playGlobalAudio(placeSounds?.getSound());
	Debug.Log(collectedObjects);
    }


}
