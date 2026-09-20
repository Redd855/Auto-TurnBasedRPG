using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Combat/Move Data")]
public class MoveData : ScriptableObject
{
    public enum MoveType
    {
        Attack,
        Heal,
        Buff,
        Debuff
    }

    public enum MoveElement
    {
        Physical,
        Fire,
        Water,
        Life,
        None
    }

    public enum StatType
    {
        PhysAttack,
        MagicAttack,
        Defense,
        None
    }

    [Header("Move Info")]
    public string moveName;

    [TextArea]
    public string description;

    [Header("Combat")]
    public MoveType moveType;
    public MoveElement moveElement;

    [Header("Effect")]
    public StatType statTarget;
    public int power;
}