/// <summary>
/// Sent from server to client
/// </summary>
public enum ServerPackets
{
    newPlayer = 1,
    welcome = 2,
    requestDenied = 4,
    playerLeft = 5,
    startGame = 6,

    playerTurn = 7,
    roomInfo = 8,
    playerEnterRoom = 9,
    playerLeaveRoom = 10,
    obtainTreasure = 11,
    hitMonster = 12,
    hitByMonster = 13,
    playerDefends = 14,
    playerLeftDungeon = 15,
    playerDies = 16,
    endGame = 17,
}

/// <summary>
/// Sent from client to server
/// </summary>
public enum ClientPackets
{
    setName = 3,

    moveRequest = 18,
    attackRequest = 19,
    defendRequest = 20,
    claimTreasureRequest = 21,
    leaveDungeonRequest = 22,
    testMessage = 69
}
