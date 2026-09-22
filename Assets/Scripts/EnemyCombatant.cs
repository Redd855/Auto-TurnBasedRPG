using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyCombatant : Combatant
{
    private CombatManager combatManager;

    public EnemyAI enemyAI;
    private void Awake()
    {
        combatManager = FindFirstObjectByType<CombatManager>();
    }

    private void Update()
    {
        if (IsDefeated())
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (hit != null && hit.gameObject == gameObject)
            {
                PlayerCombatant player = combatManager.GetCurrentPlayer();

                if (player != null && player.IsTargetingEnemy())
                {
                    player.SelectEnemy(this);
                }
            }
        }
    }

    public void TakeTurn()
    {
        enemyAI.TakeTurn();
    }
}