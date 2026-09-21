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

        if (selectedMove.targetType == MoveData.TargetType.AllEnemies)
        {
            AttackAllPlayers(selectedMove);
        }
        else
        {
            AttackOnePlayer(selectedMove);
        }

        enemy.ReduceModifierDurations();
        combatManager.NextTurn();
    }

    private void AttackOnePlayer(MoveData move)
    {
        PlayerCombatant target = combatManager.playerParty[
            Random.Range(0, combatManager.playerParty.Count)
        ];

        target.TakeDamage(enemy, move);

        Debug.Log(
            "Enemy uses " + move.moveName +
            " on " + target.gameObject.name
        );
    }

    private void AttackAllPlayers(MoveData move)
    {
        Debug.Log(
            "Enemy uses " + move.moveName +
            " on all players!"
        );

        foreach (PlayerCombatant target in combatManager.playerParty)
        {
            target.TakeDamage(enemy, move);
        }
    }
}