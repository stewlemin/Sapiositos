
/*
wallSlot goes on an empty GameObject with a BoxCollider that is set as a trigger. 
It moves an object that is behind the wall to be in front of if when Place() is called.

Place() should be called by Matei's Interact script when a raycast hits the slot
*/

using UnityEngine;

public class ArtefactSlot: MonoBehaviour
{
    [SerializeField] DoorManager door;
    [SerializeField] GameObject artefactMesh;

    public void Start(){
	artefactMesh.SetActive(false);
    }

    private bool isFilled = false;

    public bool IsFilled()
    {
        return isFilled;
    }

    public bool TryPlace(int currentNumCollected)
    {

        if (isFilled)
        {
            return false;
        }
	else if(currentNumCollected > 0){
	    isFilled = true;
	    artefactMesh.SetActive(true);
	    InteractSystem.instance.decrementCollectedObjects(gameObject.transform.position);
	    door.ArtifactPlaced();
	    return true;

	}

	return false;
    }

}
