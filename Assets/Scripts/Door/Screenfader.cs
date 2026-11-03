using System.Collections;
using UnityEngine;
 
//put this on the fade CanvasGroup (the same one GameManager uses)
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance {get; private set;}
    private const float fullFade = 1f;
    private const float fullClear = 0f;
    [SerializeField] float fadeSpeed = 2f; //tick in screen you load into
    [SerializeField] bool fadeInOnStart = false;
    private CanvasGroup group;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        group = GetComponent<CanvasGroup>();
    }

    IEnumerator Start()
    {
        if (!fadeInOnStart) yield break;
        group.alpha = fullFade;
        yield return FadeIn();
    }

    public IEnumerator FadeIn()
    {
        return FadeTo(fullClear);
    }

    public IEnumerator FadeOut()
    {
        return FadeTo(fullFade);
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        while (!Mathf.Approximately(group.alpha, targetAlpha))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }
}