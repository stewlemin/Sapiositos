using UnityEngine;

public class MonsterContext : MonoBehaviour
{
    public Transform playerTransf;
    public Camera playerCam;
    //patroller cave progress
    public Transform caveStart;
    public Transform caveExit;
    public PlayerController playerController;

    private Vector3 lastPos;
    private float speed;

    void Start()
    {
        if (playerTransf != null)
        {
            lastPos = playerTransf.position;
            if (playerController == null) playerController = playerTransf.GetComponent<PlayerController>();
        }
        if (playerController == null) Debug.LogError("monster context has no PlayerController", this);
    }

    void Update()
    {
        //calculate speed manually
        if (playerTransf != null)
        {
            speed = Vector3.Distance(playerTransf.position, lastPos) / Time.deltaTime;
            lastPos = playerTransf.position;
        }
    }

    public float GetPlayerSpeed()
    {
        return speed;
    }

    public float GetPlayerProgress()
    {
        if (caveStart == null || caveExit == null || playerTransf == null) return 0f;
        float totalDist = Vector3.Distance(caveStart.position, caveExit.position);
        if (Mathf.Approximately(totalDist, 0f)) return 0f; //guardrail if cavestart and exit are at same spot
        float currentDist = Vector3.Distance(caveStart.position, playerTransf.position);

        float progress = currentDist/ totalDist; //progress %
        return Mathf.Clamp01(progress);
    }

    public bool IsPlayerTryingToMove()
    {
        if (playerController == null) return false;
        return playerController.isPressingKey;
    }
}
