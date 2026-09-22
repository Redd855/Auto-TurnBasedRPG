using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackerAI : EnemyAI
{
    public override void TakeTurn()
    {
        List<MoveData> attackingMoves = enemy.characterData.defaultMoves.Where(move => move.moveType == MoveData.MoveType.Attack).ToList();

        if (attackingMoves.Count == 0)
        {
            Debug.LogWarning("Enemy has no attacking moves!");
            combatManager.NextTurn();
            return;
        }

        MoveData selectedMove;

        if (enemy.HasStatus(Combatant.StatusEffect.Confusion))
        {
            selectedMove = attackingMoves[Random.Range(0, attackingMoves.Count)];

            Debug.Log(enemy.gameObject.name +" is Confused and randomly selected " + selectedMove.moveName);
        }
        else
        {
            selectedMove = attackingMoves[Random.Range(0, attackingMoves.Count)];
        }

        switch (selectedMove.targetType)
        {
            case MoveData.TargetType.SingleEnemy:
                AttackOnePlayer(selectedMove);
                break;

            case MoveData.TargetType.AllEnemies:
                AttackAllPlayers(selectedMove);
                break;

            default:
                Debug.LogWarning(enemy.gameObject.name +" has an invalid attack target type: " +selectedMove.targetType);
                break;
        }

        enemy.OnTurnEnd();
        combatManager.NextTurn();
    }

    private void AttackOnePlayer(MoveData move)
    {
        PlayerCombatant target = combatManager.playerParty[Random.Range(0, combatManager.playerParty.Count)];

        target.TakeDamage(enemy, move);

        Debug.Log("Enemy uses " + move.moveName +" on " + target.gameObject.name
        );
    }

    private void AttackAllPlayers(MoveData move)
    {
        Debug.Log("Enemy uses " + move.moveName +" on all players!");

        foreach (PlayerCombatant target in combatManager.playerParty)
        {
            target.TakeDamage(enemy, move);
        }
    }
}