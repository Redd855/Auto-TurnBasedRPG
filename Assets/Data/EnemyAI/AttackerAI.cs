using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackerAI : EnemyAI
{
    public override void TakeTurn()
    {
        List<MoveData> attackingMoves = enemy.characterData.moves
            .Where(move => move.moveType == MoveData.MoveType.Attack)
            .ToList();

        if (attackingMoves.Count == 0)
        {
            Debug.LogWarning("Enemy has no attacking moves!");
            combatManager.NextTurn();
            return;
        }

        MoveData selectedMove = attackingMoves[
            Random.Range(0, attackingMoves.Count)
        ];

        PlayerCombatant target = combatManager.playerParty[
            Random.Range(0, combatManager.playerParty.Count)
        ];

        target.TakeDamage(enemy, selectedMove);

        Debug.Log("Enemy uses " + selectedMove.moveName +
                  " on " + target.gameObject.name);

        enemy.ReduceModifierDurations();
        combatManager.NextTurn();
    }
}