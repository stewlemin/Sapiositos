using UnityEngine;
using System;

[Serializable]
public class SpawnEntry
{
    public int artefactNr;
    public bool shouldSpawn;
    public float minDist;
    public float maxDist;
}

[CreateAssetMenu(menuName = "PPBMAF/Spawn Schedule")]
public class StalkerSpawnSchedule : ScriptableObject
{
    public SpawnEntry[] entries;
}
