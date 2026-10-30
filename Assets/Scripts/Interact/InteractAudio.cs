using UnityEngine;
using System.Collections;

public class InteractAudio : MonoBehaviour
{
    public static InteractAudio instance {get ; private set;}
    private GameObject audioPlayer;

    void Awake()
    {
	if(instance != null && instance != this){
	    Destroy(this);
	}
	else{
	    InteractAudio.instance = this;
	}
	audioPlayer = new GameObject();
	audioPlayer.AddComponent<AudioSource>();

    }

    public void playAudioAtLocation(AudioClip clip, Vector3 transform){
	GameObject audioClone = Instantiate(audioPlayer , transform, Quaternion.identity);
	AudioSource audioSource = audioClone.GetComponent<AudioSource>();
	audioSource.spatialBlend = 1.0f;
	audioSource.clip = clip;
	if (clip == null) return;
	audioSource.Play();
	StartCoroutine(WaitForLength(clip.length,audioSource.gameObject));
    }

    public void playAudioInChildObject(AudioClip clip, GameObject parent){

    }

    public void playGlobalAudio(AudioClip clip){
    }

    IEnumerator WaitForLength(float clipLength,GameObject toDestroy){
	yield return new WaitForSeconds(clipLength + 0.2f);	
	Destroy(toDestroy);

    }
}
