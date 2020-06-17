[System.Serializable]
public struct Room
{
    public Direction possibleDirections;
    public ushort treasureAmount;
    public bool containsMonster;
    public bool containsExit;
    public byte numberOfOtherPlayers;
    public int[] otherPlayerIds;

    //generation bools
    public bool isGap;
}
