using System.Collections.Generic;

[System.Serializable]
public class PlayerCharacter
{
    public CharacterData characterData;

    public int level = 1;
    public int experience = 0;

    public MoveData[] moves = new MoveData[3];
}