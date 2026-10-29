/*
 w *allAction keeps count of how many items have been placed. Once all of them are placed.

 I put a placeholder for when the cave is opened
 */

using UnityEngine;

// Put this on the wall
public class DoorManager : MonoBehaviour
{

    public void OnEnable(){
//        InteractSystem.instance.OnArtefactPlaced += ArtifactPlaced;
    }

    public void OnDisable(){
//        InteractSystem.instance.OnArtefactPlaced -= ArtifactPlaced;
    }

    private int numberOfArtefacts;
    private int placedArtefacts = 0;


    void Start()
    {
        numberOfArtefacts = InteractSystem.instance.getCollectibleCount();
    }

    public void ArtifactPlaced()
    {
        placedArtefacts += 1;

        if (AllPlaced())
        {
            gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }


    public bool AllPlaced()
    {
        return placedArtefacts >= numberOfArtefacts;
    }
}
