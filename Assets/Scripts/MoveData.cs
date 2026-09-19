using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Combat/Move Data")]
public class MoveData : ScriptableObject
{
    public enum MoveType
    {
        Attack,
        Heal,
        Buff
    }

    [Header("Move Info")]
    public string moveName;

    [TextArea]
    public string description;

    [Header("Combat")]
    public MoveType moveType;
    public int power;
}