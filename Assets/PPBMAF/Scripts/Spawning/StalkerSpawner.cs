using UnityEngine;

public class StalkerSpawner : MonoBehaviour
{
    public MonsterBrain stalkerBrain;
    public StalkerSpawnSchedule schedule;
    public MonsterEventChannel events;
    public MonsterContext context;
    public float spawnSpace = 1.0f;

    void OnEnable()
    {
        events.OnArtefactCollected += HandleArtefactCollected;
    }

    void OnDisable()
    {
        events.OnArtefactCollected -= HandleArtefactCollected;

    }

    //handler for artefact pickup
    private void HandleArtefactCollected(int artefactNr)
    {
        //look in every entry in schedule table
        foreach (SpawnEntry entry in schedule.entries)
        {
            //find entry that matches
            if (entry.artefactNr == artefactNr)
            {
                //find spot to spawn monster
                if (entry.shouldSpawn == true)
                {
                    Vector3 spawnSpot = FindSpawnSpot(entry.minDist, entry.maxDist);
                    stalkerBrain.SpawnMonster(spawnSpot);
                }
                break; //stop searching list
            }
        }
    }

    //helper function to find a spawn spot outside player view
    private Vector3 FindSpawnSpot(float mini, float maxi)
    {
        if (context.playerCam == null || context.playerTransf == null) return Vector3.zero;

        Transform cam = context.playerCam.transform;
        //get dir pointing backward from camera
        Vector3 behindDir = -cam.forward;
        behindDir.Normalize();

        Vector3 fallback = context.playerTransf.position;
        //try 30 times to find a spot that isnt inside the wall
        for (int i = 0; i < 30; i++)
        {
            //pick random dist btwn min and max
            float randomDist = Random.Range(mini, maxi);
            //add random angle so its not always spawning in a straight line
            float randomAng = Random.Range(-45f, 45f);
            Vector3 finalDir = Quaternion.Euler(0, randomAng, 0) * behindDir;
            Vector3 testPos = context.playerTransf.position + (finalDir * randomDist);

            if (!Physics.CheckSphere(testPos, spawnSpace, stalkerBrain.profile.obstacles)) //check sphere true if hit wall, false if not
            {
                return testPos; //clear spot
            }
            fallback = testPos; //save in case we dont find another good one
        }
        //if player is backed in corner and fails 30 times, return last try
        return fallback;
    }
}
