using UnityEngine;
using System.Collections.Generic;

public class InteractSystem : MonoBehaviour
{
    public static InteractSystem instance {get; private set;}

    //list containining references to objects, exposed in inspector
    [SerializeField] private List<GameObject> collectibles = new List<GameObject>();
    
    //number of collected objects
    private int collectedObjects = 0;

    //use "InteractSystem.instance.OnArtefactPickup += YourHandled" to subscribe to this event
    public delegate void PickUpEvent(int artefactNumber);
    public event PickUpEvent OnArtefactPickup;

    
    void Awake(){
	if(instance != null && instance != this){
	    Destroy(this);
	}
	else{
	    InteractSystem.instance = this;
	}
	for(int i = 0; i < collectibles.Count; i++){
	    Debug.Log(collectibles[i].layer);
	}
	Debug.Log(getCollectibleCount());


    }

    public int getCollectibleCount(){
	return collectibles.Count;
    }

    public bool listContainsCollectible (GameObject collectible){
	return collectibles.Contains(collectible);
    }


    public void Collect(GameObject objectToRemove){
	collectibles.Remove(objectToRemove);
	Destroy(objectToRemove);
	collectedObjects += 1;
	Debug.Log(collectedObjects);
    }
    

    public void Place(GameObject objectToTryPlace){

	
    }


}
