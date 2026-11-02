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
    public static void BroadcastArtefactCollected(int artefactNr)
    {
        if (OnArtefactCollected != null) OnArtefactCollected(artefactNr);
    }
    public void BroadcastGazeStarted()
    {
        if (OnGazeEnded != null) OnGazeStarted();
    }
    public void BroadcastGazeEnded()
    {
        if (OnGazeEnded != null) OnGazeEnded();
    }
    public void BroadcastJumpscare()
    {
        if (OnJumpscareTriggered != null) OnJumpscareTriggered();
    }
    //patroller broadcasts
    public void BroadcastSoundStarted()
    {
        if (OnSoundStarted != null) OnSoundStarted();
    }
    public void BroadcastSoundEnded()
    {
        if (OnSoundEnded != null) OnSoundEnded();
    }
    public void BroadcastWarningStarted()
    {
        if (OnWarningStarted != null) OnWarningStarted();
    }
}
