using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    [SerializeField] public List<PlayerCombatant> playerParty;
    [SerializeField] public List<EnemyCombatant> enemyParty;

    private bool playerTurn = true;
    private int currentIndex = 0;

    private void Start()
    {
        playerParty[0].ShowMoves();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextTurn();
        }
    }

    public void NextTurn()
    {
        if (playerTurn)
        {
            playerParty[currentIndex].HideMoves();
        }

        currentIndex++;

        if (playerTurn && currentIndex >= playerParty.Count)
        {
            playerTurn = false;
            currentIndex = 0;
        }
        else if (!playerTurn && currentIndex >= enemyParty.Count)
        {
            playerTurn = true;
            currentIndex = 0;
        }

        if (playerTurn)
        {
            PlayerCombatant player = playerParty[currentIndex];

            if (player.IsStunned())
            {
                Debug.Log(
                    player.gameObject.name +
                    " is stunned and skips their turn!"
                );

                player.ReduceStatusDuration();
                NextTurn();
                return;
            }

            player.ShowMoves();
        }
        else
        {
            EnemyCombatant enemy = enemyParty[currentIndex];

            if (enemy.IsStunned())
            {
                Debug.Log(
                    enemy.gameObject.name +
                    " is stunned and skips their turn!"
                );

                enemy.ReduceStatusDuration();
                NextTurn();
                return;
            }

            enemy.TakeTurn();
        }
    }

    public PlayerCombatant GetCurrentPlayer()
    {
        if (playerTurn)
            return playerParty[currentIndex];

        return null;
    }

    public bool IsAlly(Combatant user, Combatant target)
    {
        if (user is PlayerCombatant)
            return target is PlayerCombatant;

        if (user is EnemyCombatant)
            return target is EnemyCombatant;

        return false;
    }

    public bool IsEnemy(Combatant user, Combatant target)
    {
        if (user is PlayerCombatant)
            return target is EnemyCombatant;

        if (user is EnemyCombatant)
            return target is PlayerCombatant;

        return false;
    }
}