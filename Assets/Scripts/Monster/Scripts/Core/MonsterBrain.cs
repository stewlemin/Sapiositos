using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    [Header("Data references")]
    public MonsterProfile profile;
    public MonsterContext context;
    public MonsterEventChannel events;

    [Header("Current state")]
    public string currentState = "Hidden"; //can be Hidden, Approach, Frozen, Attack, TeleportAway
    
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
        string nextDecision = "Hidden";
        //ask correct rule what to do
        if (isStalker)
        {
            nextDecision = gazeRule.CheckRule(transform, Time.deltaTime);
        }
        else
        {
            nextDecision = soundRule.CheckRule(transform, Time.deltaTime);
        }
        //despawn if hidden
        if (nextDecision == "Hidden")
        {
            currentState = "Hidden";
           
            if (isStalker)
            {
                gameObject.SetActive(false); //turn off stalker, sapwner will wake it
            }
            else
            {
                transform.position = new Vector3(0, -500f, 0); //hide patroller under floor
            }

            return;
        }

        //handle stalker teleport
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
        else if (currentState == "Patrol")
        {
            //patroller walk to sweep target
            transform.position = Vector3.MoveTowards(transform.position, sweepTargetPos, profile.walkSpeed * Time.deltaTime);
            //if it reahces end of line early, hide it
            if (Vector3.Distance(transform.position, sweepTargetPos) < 1.0f)
            {
                transform.position = new Vector3(0, -500f, 0);
            }
        }
        else if (currentState == "Attack")
        {   
            gameObject.SetActive(true); //make sure its visible
            //rush to player
            Vector3 targetPos = context.playerTransf.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, profile.attackSpeed * Time.deltaTime);
            
            //if we touch or get very close to player, jumpscare
            float distToPlayer = Vector3.Distance(transform.position, targetPos);
            //PLEASE STOP WRITING NON VARIABLE COMPARISONS THIS MAKES IT EXTREMELT CONFUSING
            Debug.Log(distToPlayer);
            if (distToPlayer < 20f)
            {
                events.BroadcastJumpscare();
                currentState = "Hidden";
                gameObject.SetActive(false);
            }
        }
    }

    //helper function for patroller to calculate the line sweep
    private void StartSweep()
    {
        float forwardDist = Random.value > 0.5f ? 15f : -15f; //decide if sweep in front or behind player
        float sideDist = Random.value > 0.5f ? 20f : -20f; //decide if start left or right

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
                transform.LookAt(context.playerTransf.position); //face the player
                return;
            }
            //if cant find a spot just stay still and look at player
            transform.LookAt(context.playerTransf.position); 
        }
        
        //turn to face player
        transform.LookAt(context.playerTransf.position);
    }
}
