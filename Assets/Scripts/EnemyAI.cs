using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    protected EnemyCombatant enemy;
    protected CombatManager combatManager;

    private void Awake()
    {
        enemy = GetComponent<EnemyCombatant>();
        combatManager = FindFirstObjectByType<CombatManager>();
    }

    public abstract void TakeTurn();
}