using UnityEngine;

[CreateAssetMenu(fileName = "SoundVariations", menuName = "Scriptable Objects/SoundVariations")]
public class SoundVariations : ScriptableObject
{
   [SerializeField] AudioClip[] soundVariations;


   public AudioClip getSound(){
	if(soundVariations.Length > 0){
	   int index = Random.Range(0,soundVariations.Length); //removed -1 because it never plays the last clip because random range excludes the max
	   return soundVariations[index];
	}
	else{
	    Debug.Log("this sound's variation list is empty");
	    return null;
	}
   }
}
