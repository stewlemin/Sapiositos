using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MonsterEventChannel", menuName = "PPBMAF/Event Channel")]
public class MonsterEventChannel : ScriptableObject
{
    public Action<int> OnArtefactCollected;
    public Action OnGazeStarted;
    public Action OnGazeEnded;
    public Action OnJumpscareTriggered;

    public void BroadcastArtefactCollected(int artefactNr)
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
}
