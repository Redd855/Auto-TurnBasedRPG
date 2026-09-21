using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Combat/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Character Info")]
    public string characterName;

    [Header("Stats")]
    public int maxHP;
    public int physATK;
    public int magicATK;
    public int defense;

    [Header("Moves")]
    public List<MoveData> moves;

    [Header("Elemental Weaknesses")]
    public List<MoveData.MoveElement> weaknesses;

    public bool IsWeakTo(MoveData.MoveElement element)
    {
        if (element == MoveData.MoveElement.None)
            return false;

        return weaknesses.Contains(element);
    }
}