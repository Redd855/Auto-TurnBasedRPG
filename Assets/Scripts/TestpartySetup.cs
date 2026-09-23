using UnityEngine;
using UnityEngine.SceneManagement;

public class TestPartySetup : MonoBehaviour
{
    [SerializeField] private CharacterData knightData;
    [SerializeField] private CharacterData mageData;
    [SerializeField] private CharacterData clericData;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("GameManager.Instance is NULL!");
            return;
        }

        if (gameManager.currentBattleParty == null ||
            gameManager.currentBattleParty.Count == 0)
        {
            CreateTestParty();
        }
    }

    private void CreateTestParty()
    {


        gameManager.party.Clear();

        PlayerCharacter knight = CreateCharacter(knightData);
        PlayerCharacter mage = CreateCharacter(mageData);
        PlayerCharacter cleric = CreateCharacter(clericData);

        gameManager.party.Add(knight);
        gameManager.party.Add(mage);
        gameManager.party.Add(cleric);

        gameManager.currentBattleParty.Clear();

        gameManager.currentBattleParty.Add(knight);
        gameManager.currentBattleParty.Add(mage);
        gameManager.currentBattleParty.Add(cleric);

        Debug.Log("Test party created!");
    }

    private PlayerCharacter CreateCharacter(CharacterData data)
    {
        PlayerCharacter character = new PlayerCharacter();

        character.characterData = data;
        character.level = 1;
        character.experience = 0;

        for (int i = 0; i < character.moves.Length && i < data.defaultMoves.Count; i++)
        {
            character.moves[i] = data.defaultMoves[i];
        }

        return character;
    }

    public void StartBattle()
    {
        SceneManager.LoadScene("Combat");
    }
}