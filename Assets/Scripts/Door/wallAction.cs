/*
wallAction keeps count of how many items have been placed. Once all of them are placed.

I put a placeholder for when the cave is opened
*/

using UnityEngine;

// Put this on the wall
public class wallAction : MonoBehaviour
{

    // Used until InteractSystem is merged
    [SerializeField] int numberOfArtifacts = 3;
    private int placedArtifacts = 0;

    void Start()
    {
        // numberOfArtifacts = InteractSystem.instance.getCollectibleCount();
    }

    public void ArtifactPlaced()
    {
        placedArtifacts += 1;

        if (AllPlaced())
        {
            Debug.Log("This is what opens the cave");
        }
    }

    public bool AllPlaced()
    {
        return placedArtifacts >= numberOfArtifacts;
    }
}
