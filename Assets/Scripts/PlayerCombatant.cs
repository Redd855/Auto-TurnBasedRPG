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
    private bool selectingAlly = false;

    private void Awake()
    {
        combatManager = FindFirstObjectByType<CombatManager>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (hit != null && hit.gameObject == gameObject)
            {
                PlayerCombatant currentPlayer = combatManager.GetCurrentPlayer();

                if (currentPlayer != null)
                {
                    currentPlayer.SelectAlly(this);
                }
            }
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
        selectedMove = characterData.moves[moveIndex];

        Debug.Log(gameObject.name + " selected " + selectedMove.moveName);

        if (selectedMove.moveType == MoveData.MoveType.Attack || selectedMove.moveType == MoveData.MoveType.Debuff)
        {
            selectingTarget = true;
            selectingAlly = false;

            HideMoves();

            Debug.Log("Select an enemy to attack.");
        }
        else if (selectedMove.moveType == MoveData.MoveType.Heal || selectedMove.moveType == MoveData.MoveType.Buff)
        {
            selectingTarget = true;
            selectingAlly = true;

            HideMoves();

            Debug.Log("Select an ally to heal.");
        }
    }

    public void SelectAlly(PlayerCombatant ally)
    {
        if (!selectingTarget || !selectingAlly)
            return;

        if (selectedMove.moveType == MoveData.MoveType.Buff)
        {
            ally.ApplyStatModifier(
                selectedMove.statTarget,
                selectedMove.moveType
            );
        }
        else if (selectedMove.moveType == MoveData.MoveType.Heal)
        {
            ally.Heal(selectedMove.power);
        }

        selectingTarget = false;
        selectingAlly = false;

        ReduceModifierDurations();
        combatManager.NextTurn();
    }

    public void SelectEnemy(EnemyCombatant enemy)
    {
        if (!selectingTarget)
            return;

        Debug.Log(gameObject.name + " used " + selectedMove.moveName +
                  " on " + enemy.gameObject.name);

        if (selectedMove.moveType == MoveData.MoveType.Debuff)
        {
            enemy.ApplyStatModifier(
                selectedMove.statTarget,
                selectedMove.moveType
            );
        }
        else if (selectedMove.moveType == MoveData.MoveType.Attack)
        {
            enemy.TakeDamage(this, selectedMove);
        }

        selectingTarget = false;

        ReduceModifierDurations();
        combatManager.NextTurn();
    }

    public bool IsTargetingEnemy()
    {
        return selectingTarget && !selectingAlly;
    }
}
