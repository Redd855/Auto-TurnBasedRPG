using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Combat/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Character Info")]
    public string characterName;

    [Header("Stats")]
    public int maxHP;
    public int maxMP;
    public int physATK;
    public int magicATK;
    public int defense;

    [Header("Basic Attack")]
    public MoveData basicAttack;

    [Header("Combat Prefabs")]
    public PlayerCombatant playerCombatPrefab;
    public EnemyCombatant enemyCombatPrefab;

    [Header("Moves")]
    public List<MoveData> defaultMoves;

    [Header("Elemental Weaknesses")]
    public List<MoveData.MoveElement> weaknesses;

    [Header("Rewards")]
    public int experienceReward;

    [Header("Passive")]
    public PassiveData passive;

    public bool IsWeakTo(MoveData.MoveElement element)
    {
        if (element == MoveData.MoveElement.None)
            return false;

        return weaknesses.Contains(element);
    }
}