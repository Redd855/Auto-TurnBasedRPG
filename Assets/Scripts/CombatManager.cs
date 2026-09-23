using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public List<PlayerCombatant> playerParty = new();
    public List<EnemyCombatant> enemyParty = new();

    [SerializeField] private Transform[] playerSpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    private bool playerTurn = true;
    private int currentIndex = 0;

    private void Start()
    {
        SpawnPlayers();

        if (playerParty.Count > 0)
        {
            playerParty[0].OnTurnStart();
            playerParty[0].ShowMoves();
        }
    }

    private void SpawnPlayers()
    {
        List<PlayerCharacter> party = GameManager.Instance.currentBattleParty;

        for (int i = 0; i < party.Count; i++)
        {
            if (i >= playerSpawnPoints.Length)
            {
                Debug.LogWarning(
                    "Not enough player spawn points for the current battle party."
                );

                break;
            }

            PlayerCharacter player = party[i];

            PlayerCombatant combatant = Instantiate(
                player.characterData.playerCombatPrefab,
                playerSpawnPoints[i].position,
                Quaternion.identity
            );

            combatant.Initialize(player);
            combatant.OnBattleStart();

            playerParty.Add(combatant);
        }
    }

    public void NextTurn()
        {
            // Check if all enemies are defeated
            bool allEnemiesDefeated = true;

            foreach (EnemyCombatant enemy in enemyParty)
            {
                if (!enemy.IsDefeated())
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }

            if (allEnemiesDefeated)
            {
                Victory();
                return;
            }

            // Check if all players are defeated
            bool allPlayersDefeated = true;

            foreach (PlayerCombatant player in playerParty)
            {
                if (!player.IsDefeated())
                {
                    allPlayersDefeated = false;
                    break;
                }
            }

            if (allPlayersDefeated)
            {
                Defeat();
                return;
            }

            if (playerTurn)
            {
                playerParty[currentIndex].HideMoves();

                currentIndex++;

                // Finished all player slots, switch to enemies
                if (currentIndex >= playerParty.Count)
                {
                    playerTurn = false;
                    currentIndex = 0;
                }
            }
            else
            {
                currentIndex++;

                // Finished all enemy slots, switch to players
                if (currentIndex >= enemyParty.Count)
                {
                    playerTurn = true;
                    currentIndex = 0;
                }
            }

            if (playerTurn)
            {
                // Skip defeated players
                while (currentIndex < playerParty.Count &&
                       playerParty[currentIndex].IsDefeated())
                {
                    currentIndex++;
                }

                // All remaining players were defeated
                if (currentIndex >= playerParty.Count)
                {
                    Defeat();
                    return;
                }

                PlayerCombatant player = playerParty[currentIndex];

                if (!player.OnTurnStart())
                {
                    player.OnTurnEnd();
                    NextTurn();
                    return;
                }

                player.ShowMoves();
            }
            else
            {
                // Skip defeated enemies
                while (currentIndex < enemyParty.Count &&
                       enemyParty[currentIndex].IsDefeated())
                {
                    currentIndex++;
                }

                // All remaining enemies were defeated
                if (currentIndex >= enemyParty.Count)
                {
                    Victory();
                    return;
                }

                EnemyCombatant enemy = enemyParty[currentIndex];

                if (!enemy.OnTurnStart())
                {
                    enemy.OnTurnEnd();
                    NextTurn();
                    return;
                }

                enemy.TakeTurn();
            }
    }


    public PlayerCombatant GetCurrentPlayer()
        {
            if (!playerTurn)
                return null;

            if (currentIndex < 0 || currentIndex >= playerParty.Count)
                return null;

            return playerParty[currentIndex];
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

    private void Victory()
    {
        int totalEXP = 0;

        foreach (EnemyCombatant enemy in enemyParty)
        {
            totalEXP += enemy.characterData.experienceReward;
        }

        foreach (PlayerCharacter player in GameManager.Instance.currentBattleParty)
        {
            player.experience += totalEXP;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }

    private void Defeat()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
    }
}