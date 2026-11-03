using System.Collections;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    //movement
    [SerializeField] Vector3 openOffset = new Vector3(0f, 4f, 0f);
    [SerializeField] float openTime = 3f;
    [SerializeField] AnimationCurve easing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    //audio
    [SerializeField] SoundVariations openSounds; //plays when starts moving
    [SerializeField] SoundVariations finishSounds; //play when it stops

    private bool isOpening = false;
    private bool isOpen = false;
    public bool IsOpen => isOpen;

    [ContextMenu("Test open")]
    public void Open()
    {
        if (isOpening || isOpen) return;
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isOpening = true;
        PlaySound(openSounds);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.TransformDirection(openOffset);
        Vector3 prevPos = startPos;
        float elapsed = 0f;

        while (elapsed < openTime)
        {
            elapsed += Time.deltaTime;
            float t = easing.Evaluate(Mathf.Clamp01(elapsed/openTime));
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            transform.position = newPos;
            prevPos = newPos;
            yield return null;
        }

        isOpening = false;
        isOpen = true;
        PlaySound(finishSounds);
    }

    private void PlaySound(SoundVariations sounds)
    {
        if (sounds == null || InteractAudio.instance == null) return;
        AudioClip clip = sounds.getSound();
        if (clip == null) return;
        InteractAudio.instance.playAudioAtLocation(clip, transform.position);
    }


}