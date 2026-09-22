using UnityEngine;
using UnityEngine.SceneManagement;

public class TestPartySetup : MonoBehaviour
{
    [SerializeField] private CharacterData knightData;
    [SerializeField] private CharacterData mageData;
    [SerializeField] private CharacterData clericData;

    private void Start()
    {
        CreateTestParty();
    }

    private void CreateTestParty()
    {
        GameManager gameManager = GameManager.Instance;

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