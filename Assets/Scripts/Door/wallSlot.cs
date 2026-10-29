/*
wallSlot goes on an empty GameObject with a BoxCollider that is set as a trigger. 
It moves an object that is behind the wall to be in front of if when Place() is called.

Place() should be called by Matei's Interact script when a raycast hits the slot
*/

using UnityEngine;

public class wallSlot : MonoBehaviour
{
    [SerializeField] wallAction wall;
    // The artifact hidden behind the wall that belongs to this slot
    [SerializeField] GameObject artifact;

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

        moveArtifact();
        wall.ArtifactPlaced();
        return true;
    }

    private void moveArtifact()
    {
        artifact.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
