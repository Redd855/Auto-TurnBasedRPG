using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    public CharacterData characterData;

    [SerializeField] protected int currentHP;

    protected virtual void Start()
    {
        currentHP = characterData.maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. HP: " + currentHP);
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > characterData.maxHP)
            currentHP = characterData.maxHP;

        Debug.Log(gameObject.name + " healed " + amount + ". HP: " + currentHP);
    }
}
