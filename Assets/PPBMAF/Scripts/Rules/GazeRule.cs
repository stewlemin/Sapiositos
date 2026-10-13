using UnityEngine;

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
    public void Setup(MonsterProfile p, MonsterContext c, MonsterEventChannel e) {}

    //handles calls from the brain every frame to tell it what to do
    public string CheckRule(Transform monsterTransf, float deltaTime)
    {
        return "Hidden";
    }

    //helper function for raycast check
    private bool IsPlayerLooking(Transform monsterTransf)
    {
        return false;
    }



}
