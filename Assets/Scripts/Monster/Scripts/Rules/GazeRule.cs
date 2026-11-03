using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GazeRule
{
    //viewport coords
    private const float viewMin = 0f;
    private const float viewMax = 1f;
    private const float inFrontOfCam = 1f;
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
        ResetTimer();
    }

    //handles calls from the brain every frame to tell it what to do
    public string CheckRule(Transform monsterTransf, float deltaTime)
    {
       bool playerIsLooking = IsPlayerLooking(monsterTransf);
       if (playerIsLooking)
        {
            return HandleLookedAt(deltaTime);
        }
       return HandleNotLookedAt();
    }

    private string HandleLookedAt(float deltaTime)
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
            return MonsterStates.Attack;
        }
        //otherwise stay still while watched
        return MonsterStates.Frozen;
    }

    private string HandleNotLookedAt()
    {
        //if player just looked away this frame
        if (isLookedAt)
        {
            isLookedAt = false;
            events.BroadcastGazeEnded();
 
            lookTimer = 0f; //reset timer
            return MonsterStates.TeleportAway;
        }
 
        //approach player
        return MonsterStates.Approach;
    }

    //helper function for raycast check
    private bool IsPlayerLooking(Transform monsterTransf)
    {
        if (context.playerCam == null) return false;

        if (!IsInView(monsterTransf.position)) return false;
        if (IsBlockedByObstacle(monsterTransf.position)) return false;
        return true;
        
    }

    private bool IsInView(Vector3 worldPos)
    {
        //take monster 3d pos to 0-1 camerascreen space
        Vector3 screenPoint = context.playerCam.WorldToViewportPoint(worldPos);
        // z>0 means in front of camera, x and y btwn 0-1 means camera inside view frame
        return screenPoint.z > inFrontOfCam && screenPoint.x > viewMin && screenPoint.x < viewMax && screenPoint.y > viewMin && screenPoint.y < viewMax;
    }

    private bool IsBlockedByObstacle(Vector3 worldPos)
    {
        //see if obstacle exists btwn camera and mosnter
        Vector3 eyePos = context.playerCam.transform.position;
        Vector3 dirToMonster = worldPos - eyePos;
        float dist = dirToMonster.magnitude;

        return Physics.Raycast(eyePos, dirToMonster, dist, profile.obstacles);
    }

    //helper function to reset timer when monster spawns
    public void ResetTimer()
    {
        lookTimer = 0f;
        isLookedAt = false;
    }

}
