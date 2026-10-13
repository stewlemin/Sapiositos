using UnityEngine;

public class MonsterContext : MonoBehaviour
{
    public Transform playerTransf;
    public Camera playerCam;
    public Rigidbody playerRb;

    public float GetPlayerSpeed()
    {
        if (playerRb == null) return 0f;
        return playerRb.linearVelocity.magnitude;
    }
}
