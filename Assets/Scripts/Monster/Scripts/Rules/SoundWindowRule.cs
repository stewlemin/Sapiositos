using UnityEngine;

public class SoundWindowRule
{
    private enum Phase { Silent, Warning, Sound }

    private MonsterProfile profile;
    private MonsterContext context;
    private MonsterEventChannel events;
    private Phase currentPhase = Phase.Silent;
    private float phaseTimer = 0f;

    public void Setup(MonsterProfile p, MonsterContext c, MonsterEventChannel e)
    {
        profile = p;
        context = c;
        events = e;
        currentPhase = Phase.Silent;
        phaseTimer = profile.maxSilentTime;
        Debug.Log("Green light");
    }

    public string CheckRule(Transform monsterTransf, float deltaTime)
    {
        phaseTimer -= deltaTime;

        switch (currentPhase)
        {
            case Phase.Silent:  return UpdateSilent();
            case Phase.Warning: return UpdateWarning();
            case Phase.Sound:   return UpdateSound();
            default:            return MonsterStates.Hidden;
        }
    }

    // green light: stay hidden
    private string UpdateSilent()
    {
        if (phaseTimer > 0f) return MonsterStates.Hidden;

        currentPhase = Phase.Warning;
        phaseTimer = profile.warningTime;
        events.BroadcastWarningStarted();
        Debug.Log("Yellow light");
        return MonsterStates.Patrol;
    }

    // yellow light: walk in
    private string UpdateWarning()
    {
        if (phaseTimer <= 0f)
        {
            currentPhase = Phase.Sound;
            phaseTimer = profile.soundTime;
            events.BroadcastSoundStarted();
            Debug.Log("Red light");
        }
        return MonsterStates.Patrol;
    }

    // red light: attack if the player moves, otherwise keep walking
    private string UpdateSound()
    {
        if (context.GetPlayerSpeed() > profile.tolerance)
        {
            Debug.Log("Game over");
            return MonsterStates.Attack;
        }

        if (phaseTimer <= 0f)
        {
            currentPhase = Phase.Silent;
            events.BroadcastSoundEnded();
            // aggression scaling: less silence the further the player is
            phaseTimer = Mathf.Lerp(profile.maxSilentTime, profile.minSilentTime, context.GetPlayerProgress());
            Debug.Log("Green light");
        }
        return MonsterStates.Patrol;
    }

    public void ResetToSilent()
    {
        currentPhase = Phase.Silent;
        phaseTimer = profile.maxSilentTime;
        Debug.Log("Green light");
    }
}