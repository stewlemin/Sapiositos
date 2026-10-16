using UnityEngine;

public class MonsterContext : MonoBehaviour
{
    public Transform playerTransf;
    public Camera playerCam;
    public Rigidbody playerRb;

    //patroller cave progress
    public Transform caveStart;
    public Transform caveExit;

    public float GetPlayerSpeed()
    {
        if (playerRb == null) return 0f;
        return playerRb.linearVelocity.magnitude;
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
