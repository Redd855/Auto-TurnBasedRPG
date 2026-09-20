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
}