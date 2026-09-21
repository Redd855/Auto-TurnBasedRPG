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

    public enum StatusEffect
    {
        None,
        Poison,
        Stun,
        Dizzy,
        Confusion
    }

    protected StatusEffect currentStatus = StatusEffect.None;
    protected int statusDuration = 0;

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


    public bool TakeDamage(Combatant attacker, MoveData move)
    {
        if (attacker.AttackMisses())
        {
            Debug.Log(
                attacker.gameObject.name +
                "'s attack missed because they are Dizzy!"
            );

            return false;
        }

        float attackMultiplier;
        float defenseMultiplier;

        if (move.damageType == MoveData.DamageType.Physical)
        {
            attackMultiplier = GetMultiplier(attacker.physAttackStatus);
        }
        else
        {
            attackMultiplier = GetMultiplier(attacker.magicAttackStatus);
        }

        defenseMultiplier = GetMultiplier(defenseStatus);

        int baseDamage;

        if (move.damageType == MoveData.DamageType.Physical)
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

        if (characterData.IsWeakTo(move.moveElement))
        {
            Debug.Log(
                gameObject.name +
                " is weak to " +
                move.moveElement +
                "!"
            );

            damage *= 2;
        }

        if (damage < 1)
            damage = 1;

        currentHP -= damage;

        Debug.Log(
            attacker.gameObject.name +
            " dealt " +
            damage +
            " damage to " +
            gameObject.name
        );

        return true;
    }


    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > characterData.maxHP)
            currentHP = characterData.maxHP;

        Debug.Log(
            gameObject.name +
            " healed " +
            amount +
            ". HP: " +
            currentHP
        );
    }


    public void ApplyStatModifier(
        MoveData.StatType stat,
        MoveData.MoveType modifier)
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
            gameObject.name +
            " " +
            stat +
            " is now " +
            modifier +
            " for 3 actions."
        );
    }


    private StatModifier GetNewModifier(
        StatModifier currentStatus,
        MoveData.MoveType modifier,
        ref int duration)
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

                Debug.Log(
                    gameObject.name +
                    "'s Phys ATK modifier wore off."
                );
            }
        }


        if (magicAttackStatus != StatModifier.Normal)
        {
            magicAttackDuration--;

            if (magicAttackDuration <= 0)
            {
                magicAttackStatus = StatModifier.Normal;
                magicAttackDuration = 0;

                Debug.Log(
                    gameObject.name +
                    "'s Magic ATK modifier wore off."
                );
            }
        }


        if (defenseStatus != StatModifier.Normal)
        {
            defenseDuration--;

            if (defenseDuration <= 0)
            {
                defenseStatus = StatModifier.Normal;
                defenseDuration = 0;

                Debug.Log(
                    gameObject.name +
                    "'s Defense modifier wore off."
                );
            }
        }
    }


    public void ApplyStatusEffect(StatusEffect effect, int duration)
    {
        if (effect == StatusEffect.None)
            return;

        currentStatus = effect;
        statusDuration = duration;

        Debug.Log(
            gameObject.name +
            " is now affected by " +
            effect +
            " for " +
            duration +
            " turns."
        );
    }


    public bool HasStatus(StatusEffect effect)
    {
        return currentStatus == effect;
    }


    public bool IsStunned()
    {
        return currentStatus == StatusEffect.Stun;
    }


    public void ReduceStatusDuration()
    {
        if (currentStatus == StatusEffect.None)
            return;

        statusDuration--;

        if (statusDuration <= 0)
        {
            Debug.Log(
                gameObject.name +
                "'s " +
                currentStatus +
                " wore off."
            );

            currentStatus = StatusEffect.None;
            statusDuration = 0;
        }
    }


    public void ProcessEndOfTurnStatus()
    {
        if (currentStatus == StatusEffect.None)
            return;

        // Poison damage
        if (currentStatus == StatusEffect.Poison)
        {
            int damage = Mathf.RoundToInt(
                characterData.maxHP * 0.05f
            );

            damage = Mathf.Max(damage, 1);

            currentHP -= damage;

            Debug.Log(
                gameObject.name +
                " took " +
                damage +
                " poison damage."
            );
        }

        // Reduce duration
        statusDuration--;

        if (statusDuration <= 0)
        {
            Debug.Log(
                gameObject.name +
                "'s " +
                currentStatus +
                " wore off."
            );

            currentStatus = StatusEffect.None;
            statusDuration = 0;
        }
    }


    public bool AttackMisses()
    {
        if (!HasStatus(StatusEffect.Dizzy))
            return false;

        return Random.Range(0f, 100f) < 25f;
    }


    public void TryApplyAdditionalEffect(MoveData move)
    {
        if (move.additionalEffect == MoveData.AdditionalEffectType.None)
            return;

        if (Random.Range(0f, 100f) >= move.effectChance)
            return;

        switch (move.additionalEffect)
        {
            case MoveData.AdditionalEffectType.Status:

                ApplyStatusEffect(
                    move.statusEffect,
                    move.statusDuration
                );

                break;

            case MoveData.AdditionalEffectType.Debuff:

                ApplyStatModifier(
                    move.debuffTarget,
                    MoveData.MoveType.Debuff
                );

                break;
        }
    }
}