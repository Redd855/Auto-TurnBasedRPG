using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Combat/Move Data")]
public class MoveData : ScriptableObject
{
    [Header("Move Info")]
    public string moveName;

    [TextArea]
    public string description;

    [Header("Combat")]
    public int power;
}