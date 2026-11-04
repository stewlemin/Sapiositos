using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    private const float fullVisible = 1f;
    private const float invisible = 0f;
    [SerializeField] AudioClip endingSound;

    [Header("Surface shot")]
    [SerializeField] Camera shotCam;
    [SerializeField] float riseH = 40f;
    [SerializeField] float riseDur = 12f;
    [SerializeField] AnimationCurve riseEase = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] Color deepColor = new Color(0.04f, 0.16f, 0.2f);
    [SerializeField] Color surfaceColor = new Color(0.8f, 0.95f, 1f);
    [SerializeField] float deepFogDensity = 0.05f;
    [SerializeField] float surfaceFogDensity = 0.01f;
    [SerializeField] CanvasGroup whiteout;
    [SerializeField, Range(0f, 1f)] float whiteoutStart = 0.7f;

    [Header("Text sequence")]
    [SerializeField] CanvasGroup[] seqs;
    [SerializeField] float fadeSec = 1.5f;
    [SerializeField] float holdSec = 2.5f;

    [Header("Deaths")]
    [SerializeField] TMP_Text deathsText;
    [SerializeField] string noDeathsText = "You were never caught! The monsters went hungry today...";
    [SerializeField] string oneDeathText = "You were only killed once! The monsters are impressed...";
    [SerializeField] string manyDeathsText = "You died {0} times."; // {0} becomes the number

    [SerializeField] string sceneToLoad;

    IEnumerator Start()
    {
        //play ending sound
        if (endingSound != null)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = endingSound;
            src.loop = true;
            src.Play();
        }

        foreach(CanvasGroup seq in seqs) SetGroup(seq, invisible, false);
        SetGroup(whiteout, invisible, false); //never block buttons
        if (deathsText != null) deathsText.text = GetDeathsLine(GameManager.deaths);
        yield return Rise();
        yield return ShowSeq();
    }

    private IEnumerator Rise()
    {
        Transform camTrans = shotCam.transform;
        Vector3 startPos = camTrans.position;
        Vector3 endPos = startPos + Vector3.up * riseH;
        float elapsed = 0f;

        while (elapsed < riseDur)
        {
            elapsed += Time.deltaTime;
            float linear = Mathf.Clamp01(elapsed/riseDur);
            float t = riseEase.Evaluate(linear);

            camTrans.position = Vector3.Lerp(startPos, endPos, t);
            //water gets brighter as rise
            Color water = Color.Lerp(deepColor, surfaceColor, t);
            shotCam.backgroundColor = water;
            RenderSettings.fogColor = water;
            RenderSettings.fogDensity = Mathf.Lerp(deepFogDensity, surfaceFogDensity, t);

            whiteout.alpha = Mathf.InverseLerp(whiteoutStart, 1f, linear);
            yield return null;
        }
    }

    private IEnumerator ShowSeq()
    {
        for (int i = 0; i < seqs.Length; i++)
        {
            bool isLast = i == seqs.Length - 1;
            if (isLast)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                seqs[i].blocksRaycasts = true;
                seqs[i].interactable = true;
            }

            yield return FadeTo(seqs[i], fullVisible);
            if (isLast) yield break;

            yield return new WaitForSeconds(holdSec);
            yield return FadeTo(seqs[i], invisible);
        }
    }

    private IEnumerator FadeTo(CanvasGroup group, float target)
    {
        float start = group.alpha;
        float elapsed = 0f;

        while (elapsed < fadeSec)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, elapsed / fadeSec);
            yield return null;
        }
        group.alpha = target;
    }

    private void SetGroup(CanvasGroup group, float alpha, bool clickable) 
    {
        group.alpha = alpha;
        group.blocksRaycasts = clickable;
        group.interactable = clickable;
    }

    private string GetDeathsLine(int deaths)
    {
        if (deaths <= 0) return noDeathsText;
        if (deaths == 1) return oneDeathText;
        return string.Format(manyDeathsText, deaths);
    }

    public void PlayAgain()
    {
        GameManager.deaths = 0;
        SceneManager.LoadScene(sceneToLoad);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
