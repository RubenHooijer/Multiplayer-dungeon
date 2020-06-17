using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Networking.Transport;

public static class ServerSend
{
    #region Setup
    private static NetworkDriver driver;
    private static NativeList<NetworkConnection> connections;

    public static void Initialize(ref NetworkDriver driver, ref NativeList<NetworkConnection> connections)
    {
        ServerSend.driver = driver;
        ServerSend.connections = connections;
    }

    #endregion


    #region Functions

    #region LobbyFunctions
    public static void Welcome(NetworkConnection toConnection, int playerId, uint hexColor)
    {
        var writer = driver.BeginSend(toConnection);
        writer.WriteUShort((ushort)ServerPackets.welcome);
        writer.WriteUInt(ServerBehaviour.MessageID);

        writer.WriteInt(playerId);
        writer.WriteUInt(hexColor);

        driver.EndSend(writer);
    }

    public static void NewPlayer(NetworkConnection exceptConnection, Player player)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i].InternalId == exceptConnection.InternalId) continue;

            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.newPlayer);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(player.id);
            writer.WriteUInt(player.hexColor);
            writer.WriteString(player.name);

            driver.EndSend(writer);
        }
    }

    public static void SendOtherPlayers(NetworkConnection toConnection)
    {
        var players = ServerBehaviour.Instance.players;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id == toConnection.InternalId) continue;

            var writer = driver.BeginSend(toConnection);
            writer.WriteUShort((ushort)ServerPackets.newPlayer);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(players[i].id);
            writer.WriteUInt(players[i].hexColor);
            writer.WriteString(players[i].name);

            driver.EndSend(writer);
        }
    }

    public static void RequestDenied(uint msgid, NetworkConnection toConnection)
    {
        var writer = driver.BeginSend(toConnection);
        writer.WriteUShort((ushort)ServerPackets.requestDenied);
        writer.WriteUInt(ServerBehaviour.MessageID);

        writer.WriteUInt(msgid);

        driver.EndSend(writer);
    }

    public static void PlayerLeft(int playerId)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.playerLeft);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(playerId);

            driver.EndSend(writer);
        }
    }

    public static void StartGame()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.startGame);
            writer.WriteUInt(ServerBehaviour.MessageID);

            driver.EndSend(writer);
        }

        ServerBehaviour.Instance.dungeon = DungeonCreator.GenerateDungeon(new UnityEngine.Vector2Int(12, 12));
        SendSpawnPoints();
    }
    #endregion

    #region GameFunctions
    public static void PlayerTurn(int turnId)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.playerTurn);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(turnId);

            driver.EndSend(writer);
        }
    }

    public static void SendSpawnPoints()
    {
        UnityEngine.Debug.Log("Sending spawn points");
        var spawnPoints = DungeonCreator.GetSpawnPoints(connections.Length, ServerBehaviour.Instance.dungeon);

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            RoomInfo(ServerBehaviour.Instance.dungeon[spawnPoints[i].x, spawnPoints[i].y], connections[i]);

            //Set the player pos
            var index = ServerBehaviour.Instance.GetPlayerWithId(connections[i].InternalId);
            var p = ServerBehaviour.Instance.players[index];
            p.pos = spawnPoints[i];
            ServerBehaviour.Instance.players[index] = p;
        }
    }

    public static void RoomInfo(Room room, NetworkConnection toConnection)
    {
        var writer = driver.BeginSend(toConnection);
        writer.WriteUShort((ushort)ServerPackets.roomInfo);
        writer.WriteUInt(ServerBehaviour.MessageID);

        writer.WriteByte((byte)room.possibleDirections);
        writer.WriteUShort(room.treasureAmount);

        writer.WriteByte(System.Convert.ToByte(room.containsMonster));
        writer.WriteByte(System.Convert.ToByte(room.containsExit));

        writer.WriteByte(room.numberOfOtherPlayers);

        for (int i = 0; i < room.numberOfOtherPlayers; i++)
        {
            writer.WriteInt(room.otherPlayerIds[i]);
        }

        driver.EndSend(writer);
    }

    public static void PlayerEnterRoom(int[] clientsInRoom, int joinedClientId)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            for (int c = 0; c < clientsInRoom.Length; c++)
            {
                if (clientsInRoom[c] != joinedClientId && clientsInRoom[c] != connections[i].InternalId) continue;

                var writer = driver.BeginSend(connections[i]);
                writer.WriteUShort((ushort)ServerPackets.playerEnterRoom);
                writer.WriteUInt(ServerBehaviour.MessageID);

                writer.WriteInt(joinedClientId);
            
                driver.EndSend(writer);
            }
        }
    }

    public static void PlayerLeaveRoom(int[] clientsInRoom, int leftClientId)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            for (int c = 0; c < clientsInRoom.Length; c++)
            {
                if (clientsInRoom[c] != leftClientId  && clientsInRoom[c] != connections[i].InternalId) continue;

                var writer = driver.BeginSend(connections[i]);
                writer.WriteUShort((ushort)ServerPackets.playerLeaveRoom);
                writer.WriteUInt(ServerBehaviour.MessageID);

                writer.WriteInt(leftClientId);

                driver.EndSend(writer);
            }
        }
    }

    public static void ObtainTreasure(ushort amount, NetworkConnection toConnection)
    {
        var writer = driver.BeginSend(toConnection);
        writer.WriteUShort((ushort)ServerPackets.obtainTreasure);
        writer.WriteUInt(ServerBehaviour.MessageID);

        writer.WriteUShort(amount);

        driver.EndSend(writer);
    }

    public static void HitMonster(int playerId, ushort damageDealt)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.hitMonster);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(playerId);
            writer.WriteUShort(damageDealt);

            driver.EndSend(writer);
        }
    }

    public static void HitByMonster(int playerId, ushort newCurrentPlayerHp)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.hitByMonster);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(playerId);
            writer.WriteUShort(newCurrentPlayerHp);

            driver.EndSend(writer);
        }
    }

    public static void PlayerDefends(int playerId, ushort newCurrentPlayerHp)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.playerDefends);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(playerId);
            writer.WriteUShort(newCurrentPlayerHp);

            driver.EndSend(writer);
        }
    }

    public static void PlayerLeftDungeon(int playerId)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.playerLeftDungeon);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(playerId);

            driver.EndSend(writer);
        }
    }

    public static void PlayerDies(int playerId)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.playerDies);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteInt(playerId);

            driver.EndSend(writer);
        }
    }

    public static void EndGame(byte numberOfScores, System.Tuple<int, ushort>[] idScorePairs)
    {
        for (int i = 0; i < connections.Length; i++)
        {
            var writer = driver.BeginSend(connections[i]);
            writer.WriteUShort((ushort)ServerPackets.endGame);
            writer.WriteUInt(ServerBehaviour.MessageID);

            writer.WriteByte(numberOfScores);

            for (int c = 0; c < numberOfScores; c++)
            {
                writer.WriteInt(idScorePairs[c].Item1);
                writer.WriteUShort(idScorePairs[c].Item2);
            }

            driver.EndSend(writer);
        }
    }


    #endregion

    #endregion
}
