using UnityEngine;

[CreateAssetMenu(
    fileName = "Iron Will",
    menuName = "Combat/Passives/Iron Will"
)]
public class IronWill : PassiveData
{
    [Range(0f, 1f)]
    public float damageReduction = 0.2f;

    public override int ModifyIncomingDamage(
        Combatant owner,
        Combatant attacker,
        MoveData move,
        int damage)
    {
        return Mathf.RoundToInt(
            damage * (1f - damageReduction)
        );
    }
}