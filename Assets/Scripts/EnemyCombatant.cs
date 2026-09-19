using UnityEngine;

public class EnemyCombatant : Combatant
{
    private CombatManager combatManager;

    private void Awake()
    {
        combatManager = FindFirstObjectByType<CombatManager>();
    }

    public void TakeTurn()
    {
        Debug.Log("Enemy Attacked");

        combatManager.NextTurn();
    }
}