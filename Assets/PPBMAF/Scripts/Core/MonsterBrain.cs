using System.Transactions;
using UnityEditor.Rendering;
using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    //data vars
    public MonsterProfile profile;
    public MonsterContext context;
    public MonsterEventChannel events;
    //state vars
    public string currentState = "Hidden"; //can be Hidden, Approach, Frozen, Attack, TeleportAway
    private GazeRule gazeRule;

    void Start()
    {
        gazeRule = new GazeRule();
        gazeRule.Setup(profile, context, events);
    }

    void Update()
    {
        if (currentState == "Hidden")
        {
            return;
        }
        //ask rule what state we should be in
        string nextDecision = gazeRule.CheckRule(transform, Time.deltaTime);

        if (nextDecision == "TeleportAway")
        {
            RepositionMonster();
            currentState = "Approach";
        }
        else
        {
            currentState = nextDecision;
        }

        MoveMonster();
    }

    //handles movement logic
    private void MoveMonster()
    {
        if (context.playerTransf == null) return;

        if (currentState == "Approach")
        {
            Vector3 targetPos = context.playerTransf.position; //get player pos

            float distToPlayer = Vector3.Distance(transform.position, targetPos);
            if (distToPlayer > profile.stopDist)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, profile.walkSpeed * Time.deltaTime); //move to the player

            }
            transform.LookAt(targetPos); //turn to face player
        }
        else if (currentState == "Attack")
        {   
            //rush to player
            Vector3 targetPos = context.playerTransf.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, profile.attackSpeed * Time.deltaTime);
            //if we touch or get very close to player, jumpscare
            float distToPlayer = Vector3.Distance(transform.position, targetPos);
            if (distToPlayer < 1.5f)
            {
                events.BroadcastJumpscare();
                currentState = "Hidden";
            }
        }
    }

    //helper function to spawn monster at a position and start hunting
    public void SpawnMonster(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        gazeRule.ResetTimer();
        currentState = "Approach";
        gameObject.SetActive(true);
    }

    //helper function to teleport monster out of view
    private void RepositionMonster()
    {
        //how far away is monster? so it stays in same range as player
        float currentDist = Vector3.Distance(transform.position, context.playerTransf.position);
        //get direction behind player cam
        Vector3 behindDir = -context.playerCam.transform.forward;
        behindDir.Normalize();
        //add random angle so its not always spawning in a straight line
        float randomAng = Random.Range(-60f, 60f);
        Vector3 finalDir = Quaternion.Euler(0, randomAng, 0) * behindDir;
        //teleport monster to new spot
        transform.position = context.playerTransf.position + (finalDir * currentDist);
        //turn to face player
        transform.LookAt(context.playerTransf.position);
    }
}
