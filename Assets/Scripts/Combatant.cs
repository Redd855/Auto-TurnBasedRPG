using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    [SerializeField] protected CharacterData characterData;

    protected int currentHP;
    protected List<MoveData> currentMoves = new();

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
    }

    public void Heal(int healAmount)
    {
        currentHP += healAmount;
    }

    public MoveData GetMove(int index)
    {
        return currentMoves[index];
    }
}
