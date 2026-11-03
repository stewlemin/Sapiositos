using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    public const float pause = 1f;
    [SerializeField] Transform player;
    [SerializeField] DoorOpener door;
    //destination
    [SerializeField] string nextSceneName; //must be added in File -> Build settings
    [SerializeField] Transform tpTarget;
    [SerializeField] UnityEvent onFadedOut; //runs while screen is black
    private bool isTransitioning = false;

    void OnTriggerStay(Collider other)
    {
        if (isTransitioning) return;
        if (!other.transform.IsChildOf(player)) return; //true for player or any other child collider
        if (door != null && !door.IsOpen) return;
        StartCoroutine(Transition());
    }

    private IEnumerator Transition()
    {
        isTransitioning = true;
        if(ScreenFader.instance != null) yield return ScreenFader.instance.FadeOut();
        onFadedOut.Invoke();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        TpPlayer();
        yield return new WaitForSeconds(pause);

        if (ScreenFader.instance != null) yield return ScreenFader.instance.FadeIn();
        isTransitioning = false;
    }

    private void TpPlayer()
    {
        if (tpTarget == null)
        {
            Debug.LogWarning("Level exit has no next scene and no tp target!!");
            return;
        }

        //char controller ovvirdes pos while its enabled
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;
        player.position = tpTarget.position;
        if (controller != null) controller.enabled = true;
    }
}
