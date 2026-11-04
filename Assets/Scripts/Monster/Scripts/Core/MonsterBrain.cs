using System.Collections;
using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    private const float sweepEndReachedDist = 1.0f;
    [Header("Patroller sweep settings")]
    [SerializeField] private float sweepSideDist = 20f;
    [SerializeField] private float sweepForwardDist = 15f;
    [SerializeField] [Range(0f, 1f)] private float frontSpawnChance = 0.8f;
    private const float coinFlip = 0.5f;
    private const int reposAttempts = 30;
    private const float reposAngleRange = 60f;
    private const float reposCheckRadius = 1.0f;

    [Header("Patroller jumpscare settings")]
    [SerializeField] AudioClip jumpscareClip;
    [SerializeField] Vector3 jumpscareOffset = new Vector3(0f, -1f, 1.5f);
    [SerializeField] float jumpscareTime = 1.5f;
    [SerializeField] float shake = 0.05f;
    [SerializeField] float jumpscareDist = 20f;
    private bool isJumpscaring;

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
        if (isJumpscaring) return;

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
        if (Vector3.Distance(transform.position, sweepTargetPos) < sweepEndReachedDist)
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
            

            if (isStalker)
            {
                StartCoroutine(StalkerJumpscare());
            }
            else
            {
                StartCoroutine(PatrollerJumpscare());
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
        float forwardDist;
        //decide if sweep in front or behind player
        if (Random.value <= frontSpawnChance)
        {
            forwardDist = sweepForwardDist;
        }
        else
        {
            forwardDist = -sweepForwardDist;
        }

        float sideDist;
        //decide if start left or right
        if (Random.value <= 0.5f)
        {
            sideDist = sweepSideDist; //50% chance to go right
        }
        else
        {
            sideDist = -sweepSideDist; //50% chance to go left
        }

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

        for (int i = 0; i < reposAttempts; i++)
        {
            //add random angle so its not always spawning in a straight line
            float randomAng = Random.Range(-reposAngleRange, reposAngleRange);
            Vector3 finalDir = Quaternion.Euler(0, randomAng, 0) * behindDir;
            Vector3 testPos = context.playerTransf.position + (finalDir * currentDist);
            
            //check for walls w/ 1 radius ball
            if(!Physics.CheckSphere(testPos, reposCheckRadius, profile.obstacles))
            {
                transform.position = testPos; //found safe spot
                break;
            }
            //if cant find a spot just stay still and look at player
        }
        
        //turn to face player
        transform.LookAt(context.playerTransf.position);
    }

    private IEnumerator PatrollerJumpscare()
    {
        isJumpscaring = true;
        currentState = MonsterStates.Hidden;
        gameObject.SetActive(true);

        events.BroadcastJumpscare(); //tells game manager to start respawn
        if (jumpscareClip != null)
        {
            AudioSource.PlayClipAtPoint(jumpscareClip, context.playerCam.transform.position, 1f);
        }

        Transform cam = context.playerCam.transform;
        float timer = 0f;
        while (timer < jumpscareTime)
        {
            timer += Time.deltaTime;

            //glue monster to cam so looking away wont let u escape and a twitch
            transform.position = cam.TransformPoint(jumpscareOffset) + Random.insideUnitSphere * shake;
            Vector3 lookTarget = cam.position;
            lookTarget.y = transform.position.y; //face player
            transform.LookAt(lookTarget);
            yield return null;
        }

        HideUnderFloor();
        soundRule.ResetToSilent(); //start next cycle fresh
        currentState = MonsterStates.Hidden;
        isJumpscaring = false;

    }

    private IEnumerator StalkerJumpscare()
    {
        isJumpscaring = true;
        events.BroadcastJumpscare();
        yield return new WaitForSeconds(jumpscareTime);
        currentState = MonsterStates.Hidden;
        gameObject.SetActive(false);
        isJumpscaring = true;
    }
}
