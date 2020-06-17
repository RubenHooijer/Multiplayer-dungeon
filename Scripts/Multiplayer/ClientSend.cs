using Unity.Networking.Transport;

public static class ClientSend
{
    #region Setup

    private static NetworkDriver driver;
    private static NetworkConnection connection;

    public static void Initialize(ref NetworkDriver driver, ref NetworkConnection connection)
    {
        ClientSend.driver = driver;
        ClientSend.connection = connection;
    }

    #endregion

    #region Functions

    #region LobbyFunctions
    public static void SetName(string name)
    {
        var writer = driver.BeginSend(connection);
        writer.WriteUShort((ushort)ClientPackets.setName);
        writer.WriteUInt(ClientBehaviour.MessageID);

        writer.WriteString(name);

        driver.EndSend(writer);
    }
    #endregion

    #region GameFunctions
    public static void MoveRequest(Direction direction)
    {
        var writer = driver.BeginSend(connection);
        writer.WriteUShort((ushort)ClientPackets.moveRequest);
        writer.WriteUInt(ClientBehaviour.MessageID);

        writer.WriteByte((byte)direction);

        driver.EndSend(writer);
    }

    public static void AttackRequest()
    {
        var writer = driver.BeginSend(connection);
        writer.WriteUShort((ushort)ClientPackets.attackRequest);
        writer.WriteUInt(ClientBehaviour.MessageID);

        driver.EndSend(writer);
    }

    public static void DefendRequest()
    {
        var writer = driver.BeginSend(connection);
        writer.WriteUShort((ushort)ClientPackets.defendRequest);
        writer.WriteUInt(ClientBehaviour.MessageID);

        driver.EndSend(writer);
    }

    public static void ClaimTreasureRequest()
    {
        var writer = driver.BeginSend(connection);
        writer.WriteUShort((ushort)ClientPackets.claimTreasureRequest);
        writer.WriteUInt(ClientBehaviour.MessageID);

        driver.EndSend(writer);
    }

    public static void LeaveDungeonRequest()
    {
        var writer = driver.BeginSend(connection);
        writer.WriteUShort((ushort)ClientPackets.leaveDungeonRequest);
        writer.WriteUInt(ClientBehaviour.MessageID);

        driver.EndSend(writer);
    }

    #endregion

    #endregion
}
