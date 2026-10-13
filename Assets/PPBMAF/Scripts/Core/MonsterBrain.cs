using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    //data vars
    public MonsterProfile profile;
    public MonsterContext context;
    public MonsterEventChannel events;
    //state vars
    public string currentState = "Hidden"; //can be Hidden, Approach, Frozen, Attack
    private GazeRule gazeRule;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //handles movement logic
    private void MoveMonster() {}
}
