using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

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
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        Debug.Log("respawn event triggered");
        //fade to black
        yield return FadeTo(fullFade);

        //find closest spawnpoint
        Transform closest = FindClosestSpawn();
        playerTransf.position = closest.position; //tp player
        yield return new WaitForSeconds(pause);

        //fade back in
        yield return FadeTo(fullClear);
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
