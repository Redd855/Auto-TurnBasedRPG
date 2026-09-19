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

        if (selectedMove.moveType == MoveData.MoveType.Attack)
        {
            selectingTarget = true;
            selectingAlly = false;

            HideMoves();

            Debug.Log("Select an enemy to attack.");
        }
        else if (selectedMove.moveType == MoveData.MoveType.Heal)
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

        Debug.Log(gameObject.name + " healed " + ally.gameObject.name);

        ally.Heal(selectedMove.power);

        selectingTarget = false;
        selectingAlly = false;

        combatManager.NextTurn();
    }

    public void SelectEnemy(EnemyCombatant enemy)
    {
        if (!selectingTarget)
            return;

        Debug.Log(gameObject.name + " attacked " + enemy.gameObject.name);

        enemy.TakeDamage(selectedMove.power);

        selectingTarget = false;
        combatManager.NextTurn();
    }

    public bool IsTargetingEnemy()
    {
        return selectingTarget && !selectingAlly;
    }
}
