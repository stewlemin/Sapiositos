using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MonsterEventChannel", menuName = "PPBMAF/Event Channel")]
public class MonsterEventChannel : ScriptableObject
{
    public static Action<int> OnArtefactCollected;
    public Action OnGazeStarted;
    public Action OnGazeEnded;
    public Action OnJumpscareTriggered;
    public Action OnSoundStarted;
    public Action OnSoundEnded;
    public Action OnWarningStarted;

    //stalker broadcasts
    public static void BroadcastArtefactCollected(int artefactNr) => OnArtefactCollected?.Invoke(artefactNr);
    public void BroadcastGazeStarted() => OnGazeStarted?.Invoke();
    public void BroadcastGazeEnded() => OnGazeEnded?.Invoke();
    public void BroadcastJumpscare() => OnJumpscareTriggered?.Invoke();
    
    //patroller broadcasts
    public void BroadcastSoundStarted() => OnSoundStarted?.Invoke();
    public void BroadcastSoundEnded() => OnSoundEnded?.Invoke();
    public void BroadcastWarningStarted() => OnWarningStarted?.Invoke();
}
