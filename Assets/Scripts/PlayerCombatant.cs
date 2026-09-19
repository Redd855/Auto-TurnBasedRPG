using UnityEngine;
using TMPro;

public class PlayerCombatant : Combatant
{
    public GameObject characterMenu;
    public CombatManager combatManager;

    [SerializeField] private TMP_Text[] moveTexts;

    private void Awake()
    {
        combatManager = FindFirstObjectByType<CombatManager>();
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
        string moveName = characterData.moves[moveIndex].moveName;

        Debug.Log(gameObject.name + " used " + moveName + ":moveIndex = " + moveIndex);

        HideMoves();
        combatManager.NextTurn();
    }
}
