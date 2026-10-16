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
        phaseTimer = profile.maxSilentTime;
    }

    public string CheckRule(Transform monsterTransf, float deltaTime)
    {
        phaseTimer -= deltaTime;

        //green light silent
        if (currentPhase == "Silent")
        {
            if (phaseTimer <= 0f)
            {
                currentPhase = "Warning";
                phaseTimer = profile.warningTime;
                events.BroadcastWarningStarted();
            }
        }
        //yellow light warning
        else if (currentPhase == "Warning")
        {
            if (phaseTimer <= 0f)
            {
                currentPhase = "Sound";
                phaseTimer = profile.soundTime;
                events.BroadcastSoundStarted();
            }
        }
        //red light sound
        else if (currentPhase == "Sound")
        {
            if (context.GetPlayerSpeed() > profile.tolerance) return "Attack";
            if (phaseTimer <= 0f)
            {
                currentPhase = "Silent";
                events.BroadcastSoundEnded();
                //aggression scaling
                float progress = context.GetPlayerProgress();
                phaseTimer = Mathf.Lerp(profile.maxSilentTime, profile.minSilentTime, progress);
            }
        }
        return "Patrol";
    }
}
