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

    public enum DamageType
    {
        None,
        Physical,
        Magic
    }

    public enum MoveElement
    {
        None,
        Fire,
        Water,
        Life
    }

    public enum TargetType
    {
        SingleAlly,
        AllAllies,
        SingleEnemy,
        AllEnemies
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
    public DamageType damageType;
    public MoveElement moveElement;
    public TargetType targetType;

    [Header("Effect")]
    public StatType statTarget;
    public int power;
}