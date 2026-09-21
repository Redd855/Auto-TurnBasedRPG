using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerCombatant : Combatant
{
    public GameObject characterMenu;
    public CombatManager combatManager;

    [SerializeField] private TMP_Text[] moveTexts;

    private MoveData selectedMove;
    private bool selectingTarget = false;

    private void Awake()
    {
        combatManager = FindFirstObjectByType<CombatManager>();
    }

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Only the player whose turn it is can select targets.
        if (combatManager.GetCurrentPlayer() != this)
            return;

        if (!selectingTarget)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit == null)
            return;

        // Check if we clicked an ally.
        PlayerCombatant ally = hit.GetComponent<PlayerCombatant>();

        if (ally != null)
        {
            SelectAlly(ally);
            return;
        }

        // Check if we clicked an enemy.
        EnemyCombatant enemy = hit.GetComponent<EnemyCombatant>();

        if (enemy != null)
        {
            SelectEnemy(enemy);
        }
    }

    public void ShowMoves()
    {
        characterMenu.SetActive(true);

        for (int i = 0; i < moveTexts.Length; i++)
        {
            moveTexts[i].text = characterData.moves[i].moveName;
        }
    }

    public void HideMoves()
    {
        characterMenu.SetActive(false);
    }

    public void SelectMove(int moveIndex)
    {
        // Make sure this character is actually taking their turn.
        if (combatManager.GetCurrentPlayer() != this)
            return;

        selectedMove = characterData.moves[moveIndex];

        Debug.Log(
            gameObject.name + " selected " +
            selectedMove.moveName
        );

        selectingTarget = true;

        HideMoves();

        if (selectedMove.targetType == MoveData.TargetType.SingleAlly ||
            selectedMove.targetType == MoveData.TargetType.AllAllies)
        {
            Debug.Log("Select an ally.");
        }
        else
        {
            Debug.Log("Select an enemy.");
        }
    }

    public void SelectAlly(PlayerCombatant ally)
    {
        if (!selectingTarget)
            return;

        if (selectedMove.targetType != MoveData.TargetType.SingleAlly &&
            selectedMove.targetType != MoveData.TargetType.AllAllies)
            return;

        if (!combatManager.IsAlly(this, ally))
            return;

        Debug.Log(
            gameObject.name + " used " +
            selectedMove.moveName + " on " +
            ally.gameObject.name
        );

        if (selectedMove.targetType == MoveData.TargetType.SingleAlly)
        {
            ApplyMove(ally);
        }
        else if (selectedMove.targetType == MoveData.TargetType.AllAllies)
        {
            foreach (PlayerCombatant player in combatManager.playerParty)
            {
                ApplyMove(player);
            }
        }

        FinishMove();
    }

    public void SelectEnemy(EnemyCombatant enemy)
    {
        if (!selectingTarget)
            return;

        if (selectedMove.targetType != MoveData.TargetType.SingleEnemy &&
            selectedMove.targetType != MoveData.TargetType.AllEnemies)
            return;

        if (!combatManager.IsEnemy(this, enemy))
            return;

        Debug.Log(
            gameObject.name + " used " +
            selectedMove.moveName + " on " +
            enemy.gameObject.name
        );

        if (selectedMove.targetType == MoveData.TargetType.SingleEnemy)
        {
            ApplyMove(enemy);
        }
        else if (selectedMove.targetType == MoveData.TargetType.AllEnemies)
        {
            foreach (EnemyCombatant target in combatManager.enemyParty)
            {
                ApplyMove(target);
            }
        }

        FinishMove();
    }

    private void ApplyMove(Combatant target)
    {
        switch (selectedMove.moveType)
        {
            case MoveData.MoveType.Attack:

                target.TakeDamage(this, selectedMove);

                break;

            case MoveData.MoveType.Heal:

                target.Heal(selectedMove.power);

                break;

            case MoveData.MoveType.Buff:

                target.ApplyStatModifier(
                    selectedMove.statTarget,
                    selectedMove.moveType
                );

                break;

            case MoveData.MoveType.Debuff:

                target.ApplyStatModifier(
                    selectedMove.statTarget,
                    selectedMove.moveType
                );

                break;
        }
    }

    private void FinishMove()
    {
        selectingTarget = false;
        selectedMove = null;

        ReduceModifierDurations();

        combatManager.NextTurn();
    }

    public bool IsTargetingEnemy()
    {
        if (!selectingTarget || selectedMove == null)
            return false;

        return selectedMove.targetType == MoveData.TargetType.SingleEnemy ||
               selectedMove.targetType == MoveData.TargetType.AllEnemies;
    }
}