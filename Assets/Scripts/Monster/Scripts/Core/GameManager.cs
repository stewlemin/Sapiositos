using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MonsterEventChannel events;
    public Transform playerTransf;
    public CanvasGroup fade;
    public Transform[] spawnPoint;
    public float fadeSpeed = 2f;
    private const float fullFade = 1f;
    private const float fullClear = 0f;
    private const float pause = 1f;
    public float jumpscareTime = 0.5f; //time before fade starts
    private bool isRespawning = false;
    public Behaviour[] disableDuringRespawn;

    void OnEnable()
    {
        events.OnJumpscareTriggered += HandlePlayerDeath;
    }

    void OnDisable()
    {
        events.OnJumpscareTriggered -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        if (isRespawning) return;   //ignore 2nd jumpscare while respawing
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        foreach (var b in disableDuringRespawn) b.enabled = true;
        //Debug.Log("respawn event triggered");

        yield return new WaitForSeconds(jumpscareTime);

        //fade to black
        yield return FadeTo(fullFade);

        //find closest spawnpoint
        Transform closest = FindClosestSpawn();
        playerTransf.position = closest.position; //tp player
        yield return new WaitForSeconds(pause);

        //fade back in
        yield return FadeTo(fullClear);
        foreach(var b in disableDuringRespawn) b.enabled = true;
        isRespawning = false;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        while (!Mathf.Approximately(fade.alpha, targetAlpha))
        {
            fade.alpha = Mathf.MoveTowards(fade.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

    private Transform FindClosestSpawn()
    {
        Transform closest = spawnPoint[0];
        float shortestDist = Vector3.Distance(playerTransf.position, closest.position);
        foreach (Transform spot in spawnPoint)
        {
            float distToSpot = Vector3.Distance(playerTransf.position, spot.position);
            if (distToSpot < shortestDist)
            {
                shortestDist = distToSpot;
                closest = spot;
            }
        }
        return closest;
    }
}
