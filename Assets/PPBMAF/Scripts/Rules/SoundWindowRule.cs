using UnityEngine;

public class SoundWindowRule
{
    private MonsterProfile profile;
    private MonsterContext context;
    private MonsterEventChannel events;
    private string currentPhase = "Silent";
    private float phaseTimer = 0f;

    public void Setup(MonsterProfile p, MonsterContext c, MonsterEventChannel e)
    {
        profile = p;
        context = c;
        events = e;
        currentPhase = "Silent";
        phaseTimer = profile.silentTime;
    }

    public string CheckRule(Transform monsterTransf, float deltaTime)
    {
        return "Frozen";
    }
}
