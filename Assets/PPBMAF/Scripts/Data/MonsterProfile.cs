using UnityEngine;

[CreateAssetMenu(fileName = "MonsterProfile", menuName = "PPBMAF/Monster Profile")]
public class MonsterProfile : ScriptableObject
{
    //movement vars
    public float walkSpeed = 3.5f;
    public float attackSpeed = 10f;
    //stalker vars
    public float timeBeforeAttack = 3.0f; //how many sec player can look?
    public float cooldownSpeed = 1.0f; //how fast timer resets when looking away?
    public LayerMask obstacles; //which layers block camera?

}
