using UnityEngine;

public abstract class PassiveData : ScriptableObject
{
    [Header("Passive Info")]
    public string passiveName;

    [TextArea]
    public string description;

    public virtual void OnBattleStart(Combatant owner)
    {
    }

    public virtual void OnTurnStart(Combatant owner)
    {
    }

    public virtual void OnTurnEnd(Combatant owner)
    {
    }

    public virtual int ModifyOutgoingDamage(
        Combatant owner,
        Combatant target,
        MoveData move,
        int damage)
    {
        return damage;
    }

    public virtual int ModifyIncomingDamage(
        Combatant owner,
        Combatant attacker,
        MoveData move,
        int damage)
    {
        return damage;
    }
}