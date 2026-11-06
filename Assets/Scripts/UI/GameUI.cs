using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public enum Counter { None, Collected, Placed } //what the subgoal counts
    public enum CompleteWhen { CounterFull, GateOpen, Never } //what moves to the next objective

    [Serializable]
    public class Objective
    {
        [TextArea] public string goal;
        public Counter subgoalCounter = Counter.None; //None means no subgoal shown
        public string subgoalFormat = "Collected: {0}/{1}"; // {0} = current, {1} = total
        public CompleteWhen completeWhen = CompleteWhen.Never;
    }

    [Serializable]
    public class Hint
    {
        [TextArea] public string text;
        public float delayBefore = 0.5f;
        public float holdTime = 4f; // time fully visible
    }

    [Header("Objectives")]
    [SerializeField] private Objective[] objectives =
    {
        new Objective 
        { 
            goal = "Collect all artefacts", 
            subgoalCounter = Counter.Collected,
            subgoalFormat = "Collected: {0}/{1}", 
            completeWhen = CompleteWhen.CounterFull 
        },
        new Objective 
        { 
            goal = "Find and open the gate", 
            subgoalCounter = Counter.Placed,    
            subgoalFormat = "Placed: {0}/{1}",    
            completeWhen = CompleteWhen.GateOpen 
        },
        new Objective 
        { 
            goal = "Go through the gate", 
            subgoalCounter = Counter.None, 
            completeWhen = CompleteWhen.Never 
        },
    };
    [SerializeField] private DoorOpener gate;
    [SerializeField] private CanvasGroup objectivesGroup; // parent of the texts below
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private TextMeshProUGUI subgoalText;
    [SerializeField] private float objectiveFade = 0.4f;

    [Header("Tutorial hints")]
    [SerializeField] private Hint[] hints =
    {
        new Hint { text = "Press WASD to move" },
        new Hint { text = "Press E to interact" },
        new Hint { text = "Press Q to switch tools" },
    };
    [SerializeField] private CanvasGroup hintGroup;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintStartDelay = 2f;
    [SerializeField] private float hintFade = 1f;

    [Header("Reticle")]
    [SerializeField] private Image reticle;
    [SerializeField] private Sprite dotSprite;
    [SerializeField] private Sprite handSprite;
    [SerializeField] private float reticleScale = 1f;
    [SerializeField] private Camera cam; 
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactMask = 0;

    private int total, collected, placed;
    private ArtefactSlot[] slots;
    private int index = -1;
    private bool objectiveReady;
    private int lastShownCount = -1;
    private bool isHand;
    private Coroutine objectiveRoutine;

    //sets up the reticle, remembers how many artefacts are, shows first objective and starts tutorial
    void Start()
    {
        if (cam == null) cam = Camera.main;
        if (InteractSystem.instance != null) total = InteractSystem.instance.getCollectibleCount(); //before anything is collected
        slots = FindObjectsByType<ArtefactSlot>(FindObjectsSortMode.None);
        reticle.raycastTarget = false;
        reticle.color = Color.white;
        SetReticle(false);

        objectivesGroup.alpha = 0f;
        if (objectives.Length > 0) ShowObjective(0);

        if (hints.Length > 0 && hintGroup != null && hintText != null)
        {
            hintGroup.alpha = 0f;
            StartCoroutine(RunHints());
        }
    }

    //every frame refresh the counts, swap the reticle sprite if needed, check if the objective is done
    void Update()
    {
        RefreshCounts();

        bool hover = IsLookingAtInteractable();
        if (hover != isHand) SetReticle(hover);

        UpdateObjective();
    }

    //recalculates how many artefacts were collected and how many are placed in slots
    private void RefreshCounts()
    {
        if (InteractSystem.instance != null)
            collected = Mathf.Clamp(total - InteractSystem.instance.getCollectibleCount(), 0, total);

        placed = 0;
        foreach (ArtefactSlot s in slots)
            if (s != null && s.IsFilled()) placed++;
    }


    //returns the current number for a counter type (used for the subgoal text)
    private int GetCount(Counter c)
    {
        switch (c)
        {
            case Counter.Collected: return collected;
            case Counter.Placed: return placed;
            default: return 0;
        }
    }

    //updates the subgoal text and moves to the next objective once the current one is complete
    private void UpdateObjective()
    {
        if (!objectiveReady) return; //wait for fade transition to swap the text

        Objective o = objectives[index];
        RefreshSubgoalText(o);

        if (IsComplete(o) && index + 1 < objectives.Length) ShowObjective(index + 1);
    }

    private bool IsComplete(Objective o)
    {
        switch (o.completeWhen)
        {
            case CompleteWhen.CounterFull:
                return o.subgoalCounter != Counter.None && total > 0 && GetCount(o.subgoalCounter) >= total;
            case CompleteWhen.GateOpen:
                return gate != null && gate.IsOpen;
            default:
                return false;
        }
    }

    //switches to objective number i (restarts the fade if one is already running)
    private void ShowObjective(int i)
    {
        index = i;
        if (objectiveRoutine != null) StopCoroutine(objectiveRoutine);
        objectiveRoutine = StartCoroutine(ObjectiveTransition());
    }

    //fades the old text out, swaps in the new goal/subgoal, fades back in
    private IEnumerator ObjectiveTransition()
    {
        objectiveReady = false;
        if (objectivesGroup.alpha > 0f) yield return Fade(objectivesGroup, 0f, objectiveFade);

        Objective o = objectives[index];
        goalText.text = o.goal;
        subgoalText.gameObject.SetActive(o.subgoalCounter != Counter.None);
        lastShownCount = -1; //force subgoal text refresh
        objectiveReady = true;
        RefreshSubgoalText(o);

        yield return Fade(objectivesGroup, 1f, objectiveFade);
    }

    private void RefreshSubgoalText(Objective o)
    {
        if (o.subgoalCounter == Counter.None) return;
        int count = GetCount(o.subgoalCounter);
        if (count == lastShownCount) return;
        lastShownCount = count;
        subgoalText.text = string.Format(o.subgoalFormat, count, total);
    }

    //reticle
    private bool IsLookingAtInteractable()
    {
        if (cam == null) return false;
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRange, interactMask, QueryTriggerInteraction.Collide)) return false;

        //slot: only interactable if empty and the player has an artefact
        ArtefactSlot slot = hit.collider.GetComponentInParent<ArtefactSlot>();
        if (slot != null) return !slot.IsFilled() && collected > placed;

        //collectible: the hit object or any parent is in InteractSystem's list
        if (InteractSystem.instance == null) return false;
        for (Transform t = hit.collider.transform; t != null; t = t.parent)
            if (InteractSystem.instance.listContainsCollectible(t.gameObject)) return true;

        return false;
    }

    //swaps between the reticle and the dot
    private void SetReticle(bool hand)
    {
        isHand = hand;
        reticle.sprite = hand ? handSprite : dotSprite;
        reticle.SetNativeSize();
        reticle.rectTransform.sizeDelta *= reticleScale;
    }

    //tutorial
    private IEnumerator RunHints()
    {
        yield return new WaitForSeconds(hintStartDelay);

        foreach (Hint h in hints)
        {
            yield return new WaitForSeconds(h.delayBefore);
            hintText.text = h.text;
            yield return Fade(hintGroup, 1f, hintFade);
            yield return new WaitForSeconds(h.holdTime);
            yield return Fade(hintGroup, 0f, hintFade);
        }
    }

    private static IEnumerator Fade(CanvasGroup group, float target, float duration)
    {
        float start = group.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        group.alpha = target;
    }
}