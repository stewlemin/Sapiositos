using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    private const float jumpscareDist = 20f;
    private const float sweepEndReachedDist = 1.0f;
    private const float sweepSideDist = 20f;
    private const float sweepForwardDist = 15f;
    private const float coinFlip = 0.5f;
    private const int reposAttempts = 30;
    private const float reposAngleRange = 60f;
    private const float reposCheckRadius = 1.0f;

    [Header("Data references")]
    public MonsterProfile profile;
    public MonsterContext context;
    public MonsterEventChannel events;

    [Header("Current state")]
    public string currentState = MonsterStates.Hidden; //can be Hidden, Approach, Frozen, Attack, TeleportAway
    
    [Header("Monster type")]
    public bool isStalker = true; //true = stalker, false = patroller
    private GazeRule gazeRule;
    private SoundWindowRule soundRule;
    private Vector3 sweepTargetPos; //where patroller is walking to

    void Start()
    {
        if (isStalker)
        {
            gazeRule = new GazeRule();
            gazeRule.Setup(profile, context, events);
        }
        else
        {
            soundRule = new SoundWindowRule();
            soundRule.Setup(profile, context, events);
            events.OnWarningStarted += StartSweep; //listen to warning event to start sweep
        }
        
    }

    void OnDisable()
    {
        if (!isStalker) events.OnWarningStarted -= StartSweep; //listener cleanup
    }

    void Update()
    {
        string nextDecision = GetDecision();
        
        //despawn if hidden
        if (nextDecision == MonsterStates.Hidden)
        {
            HandleHidden();
            return;
        }

        //handle stalker teleport
        if (nextDecision == MonsterStates.TeleportAway)
        {
            RepositionMonster();
            currentState = MonsterStates.Approach;
        }
        else
        {
            currentState = nextDecision;
        }

        MoveMonster();
    }

    private string GetDecision()
    {
        //ask correct rule what to do
        if (isStalker)
        {
            return gazeRule.CheckRule(transform, Time.deltaTime);
        }
        else
        {
            return soundRule.CheckRule(transform, Time.deltaTime);
        }
    }

    private void HandleHidden()
    {
        currentState = "Hidden";
           
            if (isStalker)
            {
                gameObject.SetActive(false); //turn off stalker, sapwner will wake it
            }
            else
            {
                HideUnderFloor();
            }

            return;
    }

    private void HideUnderFloor()
    {
        transform.position = new Vector3(0, -500f, 0);
    }

    //handles movement logic
    private void MoveMonster()
    {
        if (context.playerTransf == null) return;

        switch (currentState)
        {
            case MonsterStates.Approach: Approach(); break;
            case MonsterStates.Patrol: Patrol(); break;
            case MonsterStates.Attack: Attack(); break;
        }
    }

    private void Approach()
    {
        Vector3 targetPos = context.playerTransf.position; //get player pos

        float distToPlayer = Vector3.Distance(transform.position, targetPos);
        if (distToPlayer > profile.stopDist)
        {
           MoveToward(targetPos, profile.walkSpeed); //move to the player
        }
        transform.LookAt(targetPos); //turn to face player
    }

    private void Patrol()
    {
        //patroller walk to sweep target
        MoveToward(sweepTargetPos, profile.walkSpeed);
        //if it reahces end of line early, hide it
        if (Vector3.Distance(transform.position, sweepTargetPos) < 1.0f)
        {
            HideUnderFloor();
        }
    }

    private void Attack()
    {
        gameObject.SetActive(true); //make sure its visible
        //rush to player
        Vector3 targetPos = context.playerTransf.position;
        MoveToward(targetPos, profile.attackSpeed);
        
        //if we touch or get very close to player, jumpscare
        float distToPlayer = Vector3.Distance(transform.position, targetPos);
        //Debug.Log(distToPlayer);
         if (distToPlayer < jumpscareDist)
        {
            events.BroadcastJumpscare();
            currentState = MonsterStates.Hidden;

            if (isStalker)
            {
                gameObject.SetActive(false); //spawner awakes stalker
            }
            else
            {
                HideUnderFloor(); //stay active so update and rule keep running
                soundRule.ResetToSilent(); //start next cycle fresh
            }
            
        }
    }

    private void MoveToward(Vector3 targetPos, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    //helper function for patroller to calculate the line sweep
    private void StartSweep()
    {
        float forwardDist = RandomSign(sweepForwardDist); //decide if sweep in front or behind player
        float sideDist = RandomSign(sweepSideDist); //decide if start left or right

        //get player orientation
        Vector3 playerPos = context.playerTransf.position;
        Vector3 pForward = context.playerTransf.forward;
        Vector3 pRight = context.playerTransf.right;

        //calculate start and endpoints in world
        Vector3 startPos = playerPos + (pForward * forwardDist) + (pRight * sideDist);
        sweepTargetPos = playerPos + (pForward * forwardDist) + (pRight * -sideDist); //opposite side
        
        transform.position = startPos; //tp to start
        transform.LookAt(sweepTargetPos); //face end target
        gameObject.SetActive(true); //turn on
    }

    //helper function that returns +/- magnitude with 50% chance
    private float RandomSign(float magnitude)
    {
        return Random.value > coinFlip ? magnitude : -magnitude;
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

        for (int i = 0; i < 30; i++)
        {
            //add random angle so its not always spawning in a straight line
            float randomAng = Random.Range(-60f, 60f);
            Vector3 finalDir = Quaternion.Euler(0, randomAng, 0) * behindDir;
            Vector3 testPos = context.playerTransf.position + (finalDir * currentDist);
            
            //check for walls w/ 1 radius ball
            if(!Physics.CheckSphere(testPos, 1.0f, profile.obstacles))
            {
                transform.position = testPos; //found safe spot
                break;
            }
            //if cant find a spot just stay still and look at player
        }
        
        //turn to face player
        transform.LookAt(context.playerTransf.position);
    }
}
