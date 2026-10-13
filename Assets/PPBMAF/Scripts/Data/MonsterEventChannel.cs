using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MonsterEventChannel", menuName = "Scriptable Objects/MonsterEventChannel")]
public class MonsterEventChannel : ScriptableObject
{
    public Action<int> OnArtefactCollected;
    public Action OnGazeStarted;
    public Action OnGazeEnded;
    public Action OnJumpscareTriggered;

    public void BroadcastArtefactCollected(int artefactNr) {}
    public void BroadcastGazeStarted() {}
    public void BroadcastGazeEnded() {}
    public void BroadcastJumpscare() {}
}
