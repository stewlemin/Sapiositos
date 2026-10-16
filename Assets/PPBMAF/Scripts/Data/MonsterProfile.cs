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
    public float silentTime = 6.0f; //time player can move
    public float warningTime = 1.5f; //grace period timer
    public float soundTime = 4.0f; //time player cant move

}
