using UnityEngine;
using UnityEngine.Rendering;

public class GazeRule
{
    //data vars
    private MonsterProfile profile;
    private MonsterContext context;
    private MonsterEventChannel events;
    //tracker vars
    private float lookTimer = 0f;
    private bool isLookedAt = false;

    //handles rule setup
    public void Setup(MonsterProfile p, MonsterContext c, MonsterEventChannel e)
    {
        profile = p;
        context = c;
        events = e;
        lookTimer = 0f;
        isLookedAt = false;
    }

    //handles calls from the brain every frame to tell it what to do
    public string CheckRule(Transform monsterTransf, float deltaTime)
    {
       bool playerIsLooking = IsPlayerLooking(monsterTransf);

       if (playerIsLooking)
        {
            //if player just started looking at monster this frame
            if (!isLookedAt)
            {
                isLookedAt = true;
                events.BroadcastGazeStarted();
            }
            //increment timer
            lookTimer += deltaTime;

            //if player stared for too long, trigger attack
            if (lookTimer >= profile.timeBeforeAttack)
            {
                return "Attack";
            }
            //otherwise stay still while watched
            return "Frozen";
        }
        else
        {   
            //if player just looked away this frame
            if (isLookedAt)
            {
                isLookedAt = false;
                events.BroadcastGazeEnded();

                lookTimer = 0f; //reset timer
                return "TeleportAway";
            }

            //approach player
            return "Approach";

        }
    }

    //helper function for raycast check
    private bool IsPlayerLooking(Transform monsterTransf)
    {
        if (context.playerCam == null) return false;

        //take monster 3d pos to 0-1 camerascreen space
        Vector3 screenPoint = context.playerCam.WorldToViewportPoint(monsterTransf.position);
        // z>0 means in front of camera, x and y btwn 0-1 means camera inside view frame
        bool inView = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;
        if (!inView) return false;

        //see if obstacle exists btwn camera and monster
        Vector3 eyePos = context.playerCam.transform.position;
        Vector3 dirToMonster = monsterTransf.position - eyePos;
        float dist = dirToMonster.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(eyePos, dirToMonster, out hit, dist, profile.obstacles)) return false;

        return true;
        
    }

    //helper function to reset timer when monster spawns
    public void ResetTimer()
    {
        lookTimer = 0f;
        isLookedAt = false;
    }

}
