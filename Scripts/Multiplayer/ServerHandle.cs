using Extensions;
using Unity.Networking.Transport;
using UnityEngine;

public static class ServerHandle
{
    #region LobbyFunctions
    public static void SetName(DataStreamReader stream, NetworkConnection connection)
    {
        stream.ReadUInt();

        var playerName = stream.ReadString();
        Debug.Log("Server: name the client has send = " + playerName);

        Player player = new Player
        {
            id = connection.InternalId,
            hexColor = ServerBehaviour.Instance.availableHexColors[connection.InternalId].ColorToUInt(),
            name = playerName.ToString()
        };

        ServerBehaviour.Instance.players.Add(player);
        Debug.Log("Server: Player I added: " +
            $"ID: {player.id}, HexColor: {player.hexColor}, Name: {player.name}");

        ServerSend.NewPlayer(connection, player);
        ServerSend.SendOtherPlayers(connection);
    }
    #endregion

    #region GameFunctions
    public static void MoveRequest(DataStreamReader stream, NetworkConnection connection)
    {
        uint msgId = stream.ReadUInt();

        Direction direction = (Direction)stream.ReadByte();
        Vector2Int vectorDirection = Vector2Int.zero;

        switch (direction)
        {
            case Direction.up:
                vectorDirection = Vector2Int.up;
                break;
            case Direction.right:
                vectorDirection = Vector2Int.right;
                break;
            case Direction.down:
                vectorDirection = Vector2Int.down;
                break;
            case Direction.left:
                vectorDirection = Vector2Int.left;
                break;
            default:
                break;
        }

        var dungeon = ServerBehaviour.Instance.dungeon;
        var index = ServerBehaviour.Instance.GetPlayerWithId(connection.InternalId);
        var p = ServerBehaviour.Instance.players[index];

        if ((dungeon[p.pos.x, p.pos.y].possibleDirections & direction) == direction)
        {
            p.pos += vectorDirection;
            ServerSend.RoomInfo(dungeon[p.pos.x, p.pos.y], connection);

            ServerBehaviour.Instance.players[index] = p;
        } else
        {
            ServerSend.RequestDenied(msgId, connection);
        }

    }

    public static void AttackRequest(DataStreamReader stream, NetworkConnection connection)
    {
        stream.ReadInt();
    }

    public static void DefendRequest(DataStreamReader stream, NetworkConnection connection)
    {
        stream.ReadInt();
    }

    public static void ClaimTreasureRequest(DataStreamReader stream, NetworkConnection connection)
    {
        stream.ReadInt();
    }

    public static void LeaveDungeonRequest(DataStreamReader stream, NetworkConnection connection)
    {
        stream.ReadInt();
    }

    #endregion
}
