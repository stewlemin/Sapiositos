using UnityEngine;

[CreateAssetMenu(fileName = "MonsterProfile", menuName = "PPBMAF/Monster Profile")]
public class MonsterProfile : ScriptableObject
{
    //movement vars
    public float walkSpeed = 3.5f;
    public float attackSpeed = 10f;
    public float stopDist = 6.0f;

    //stalker vars
    public float timeBeforeAttack = 3.0f; //how many sec player can look?
    public float cooldownSpeed = 1.0f; //how fast timer resets when looking away?
    public LayerMask obstacles; //which layers block camera?

    //patroller vars
    public float maxSilentTime = 6.0f; //time player can move at the start
    public float minSilentTime = 6.0f; //time player can move at the end
    public float warningTime = 1.5f; //grace period timer
    public float soundTime = 4.0f; //time player cant move
    public float tolerance = 0.05f; //movement physics tolerance
}
