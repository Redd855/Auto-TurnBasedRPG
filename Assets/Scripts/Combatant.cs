using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    public CharacterData characterData;

    [SerializeField] protected int currentHP;

    public enum StatModifier
    {
        Normal,
        Buff,
        Debuff
    }
    protected StatModifier physAttackStatus = StatModifier.Normal;
    protected StatModifier magicAttackStatus = StatModifier.Normal;
    protected StatModifier defenseStatus = StatModifier.Normal;

    protected int physAttackDuration = 0;
    protected int magicAttackDuration = 0;
    protected int defenseDuration = 0;


    protected virtual void Start()
    {
        currentHP = characterData.maxHP;
    }

    public void TakeDamage(Combatant attacker, MoveData move)
    {
        float attackMultiplier;
        float defenseMultiplier;

        if (move.moveElement == MoveData.MoveElement.Physical)
        {
            attackMultiplier = GetMultiplier(attacker.physAttackStatus);
        }
        else
        {
            attackMultiplier = GetMultiplier(attacker.magicAttackStatus);
        }

        defenseMultiplier = GetMultiplier(defenseStatus);

        int baseDamage;

        if (move.moveElement == MoveData.MoveElement.Physical)
        {
            baseDamage = attacker.characterData.physATK + move.power;
        }
        else
        {
            baseDamage = attacker.characterData.magicATK + move.power;
        }

        int damage = Mathf.RoundToInt(
            baseDamage * attackMultiplier / defenseMultiplier
        );

        if (damage < 1)
            damage = 1;

        currentHP -= damage;

        Debug.Log(
            attacker.gameObject.name + " dealt " +
            damage + " damage to " +
            gameObject.name
        );
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > characterData.maxHP)
            currentHP = characterData.maxHP;

        Debug.Log(gameObject.name + " healed " + amount + ". HP: " + currentHP);
    }

    public void ApplyStatModifier(MoveData.StatType stat, MoveData.MoveType modifier)
    {
        switch (stat)
        {
            case MoveData.StatType.PhysAttack:
                physAttackStatus = GetNewModifier(
                    physAttackStatus,
                    modifier,
                    ref physAttackDuration
                );
                break;

            case MoveData.StatType.MagicAttack:
                magicAttackStatus = GetNewModifier(
                    magicAttackStatus,
                    modifier,
                    ref magicAttackDuration
                );
                break;

            case MoveData.StatType.Defense:
                defenseStatus = GetNewModifier(
                    defenseStatus,
                    modifier,
                    ref defenseDuration
                );
                break;
        }

        Debug.Log(
            gameObject.name + " " + stat +
            " is now " + modifier +
            " for 3 actions."
        );
    }

    private StatModifier GetNewModifier(StatModifier currentStatus,MoveData.MoveType modifier,ref int duration)
    {
        if (modifier == MoveData.MoveType.Buff)
        {

            if (currentStatus == StatModifier.Debuff)
            {
                duration = 0;
                return StatModifier.Normal;
            }


            if (currentStatus == StatModifier.Buff)
            {
                duration = 3;
                return StatModifier.Buff;
            }

            duration = 3;
            return StatModifier.Buff;
        }

        if (modifier == MoveData.MoveType.Debuff)
        {

            if (currentStatus == StatModifier.Buff)
            {
                duration = 0;
                return StatModifier.Normal;
            }


            if (currentStatus == StatModifier.Debuff)
            {
                duration = 3;
                return StatModifier.Debuff;
            }

            duration = 3;
            return StatModifier.Debuff;
        }

        return currentStatus;
    }
    private float GetMultiplier(StatModifier status)
    {
        switch (status)
        {
            case StatModifier.Buff:
                return 1.5f;

            case StatModifier.Debuff:
                return 0.75f;

            default:
                return 1.0f;
        }
    }

    public void ReduceModifierDurations()
    {
        if (physAttackStatus != StatModifier.Normal)
        {
            physAttackDuration--;

            if (physAttackDuration <= 0)
            {
                physAttackStatus = StatModifier.Normal;
                physAttackDuration = 0;

                Debug.Log(gameObject.name + "'s Phys ATK modifier wore off.");
            }
        }

        if (magicAttackStatus != StatModifier.Normal)
        {
            magicAttackDuration--;

            if (magicAttackDuration <= 0)
            {
                magicAttackStatus = StatModifier.Normal;
                magicAttackDuration = 0;

                Debug.Log(gameObject.name + "'s Magic ATK modifier wore off.");
            }
        }

        if (defenseStatus != StatModifier.Normal)
        {
            defenseDuration--;

            if (defenseDuration <= 0)
            {
                defenseStatus = StatModifier.Normal;
                defenseDuration = 0;

                Debug.Log(gameObject.name + "'s Defense modifier wore off.");
            }
        }
    }
}
