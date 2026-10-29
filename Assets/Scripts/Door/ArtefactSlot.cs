
/*
wallSlot goes on an empty GameObject with a BoxCollider that is set as a trigger. 
It moves an object that is behind the wall to be in front of if when Place() is called.

Place() should be called by Matei's Interact script when a raycast hits the slot
*/

using UnityEngine;

public class ArtefactSlot: MonoBehaviour
{
    [SerializeField] DoorManager door;

    private bool isFilled = false;

    public bool IsFilled()
    {
        return isFilled;
    }

    public bool Place()
    {
        if (isFilled)
        {
            return false;
        }
        isFilled = true;

        door.ArtifactPlaced();
        return true;
    }

}
