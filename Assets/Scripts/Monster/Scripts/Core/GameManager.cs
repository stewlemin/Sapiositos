using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MonsterEventChannel events;
    public Transform playerTransf;
    public CanvasGroup fade;
    public Transform[] spawnPoint;
    public float fadeSpeed = 2f;

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
        Debug.Log("respawn event triggereda");
        //fade to black
        while (fade.alpha < 1f)
        {
            fade.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        //find closest spawn point
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
        playerTransf.position = closest.position; //tp player
        yield return new WaitForSeconds(1f);
        //fade back in
        while(fade.alpha > 0f)
        {
            fade.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
