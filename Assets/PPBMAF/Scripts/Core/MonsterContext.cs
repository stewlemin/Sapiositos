using UnityEngine;

public class MonsterContext : MonoBehaviour
{
    public Transform playerTransf;
    public Camera playerCam;
    public Rigidbody playerRb;

    //patroller cave progress
    public Transform caveStart;
    public Transform caveExit;

    private Vector3 lastPos;
    private float speed;

    void Start()
    {
        if (playerTransf != null)
        {
            lastPos = playerTransf.position;
        }
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
        float currentDist = Vector3.Distance(caveStart.position, playerTransf.position);

        float progress = currentDist/ totalDist; //progress %
        return Mathf.Clamp01(progress);
    }
}
