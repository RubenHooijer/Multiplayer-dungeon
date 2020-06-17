using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public static class ClientHandle
{
    //Behaviour Actions
    public static Action OnDisconnect;

    //Lobby Actions
    public static Action<Player> WelcomeReceived;
    public static Action<Player> NewPlayerReceived;
    public static Action<ushort> RequestDeniedReceived;
    public static Action<int> PlayerLeftLobbyReceived;
    public static Action StartGameReceived;

    //Game Actions
    public static Action<int> PlayerTurnReceived;
    public static Action<Room> RoomInfoReceived;
    public static Action<int> PlayerEnterRoomReceived;
    public static Action<int> PlayerLeaveRoomReceived;
    public static Action<ushort> ObtainTreasureReceived;
    public static Action<int, ushort> HitMonsterReceived;
    public static Action<int, ushort> HitByMonsterReceived;
    public static Action<int, ushort> PlayerDefendsReceived;
    public static Action<int> PlayerLeftDungeonReceived;
    public static Action<int> PlayerDiesReceived;
    public static Action<List<Tuple<int, ushort>>> EndGameReceived;

    #region LobbyFunctions
    public static void Welcome(DataStreamReader stream)
    {
        stream.ReadUInt();

        int myPlayerId = stream.ReadInt();
        uint myPlayerColor = stream.ReadUInt();

        Debug.Log($"Client: I have received {myPlayerId} as ID and {myPlayerColor} as my color");

        Player p = new Player
        {
            id = myPlayerId,
            hexColor = myPlayerColor
        };

        WelcomeReceived?.Invoke(p);
    }

    public static void NewPlayer(DataStreamReader stream)
    {
        stream.ReadUInt();

        Player p = new Player
        {
            id = stream.ReadInt(),
            hexColor = stream.ReadUInt(),
            name = stream.ReadString().ToString()
        };

        Debug.Log("Client: Player we got sent: " +
            $"ID: {p.id}, HexColor: {p.hexColor}, Name: {p.name}");

        NewPlayerReceived?.Invoke(p);


    }

    public static void RequestDenied(DataStreamReader stream)
    {
        stream.ReadUInt();

        uint msgId = stream.ReadUInt();

        Debug.Log("Client: Request denied");
    }

    public static void PlayerLeft(DataStreamReader stream)
    {
        stream.ReadUInt();

        int leftPlayerId = stream.ReadInt();

        //GameManager.Instance.RemovePlayer(leftPlayerId);
        PlayerLeftLobbyReceived?.Invoke(leftPlayerId);
    }

    public static void StartGame(DataStreamReader stream)
    {
        stream.ReadUInt();
        Debug.Log("Client: Game has started");
        StartGameReceived?.Invoke();
    }
    #endregion

    #region GameFunctions
    public static void PlayerTurn(DataStreamReader stream)
    {
        stream.ReadUInt();

        int turnId = stream.ReadInt();
        PlayerTurnReceived?.Invoke(turnId);
    }

    public static void RoomInfo(DataStreamReader stream)
    {
        stream.ReadUInt();

        Room cRoom = new Room()
        {
            possibleDirections = (Direction)stream.ReadByte(),
            treasureAmount = stream.ReadUShort(),

            containsMonster = System.Convert.ToBoolean(stream.ReadByte()),
            containsExit = System.Convert.ToBoolean(stream.ReadByte()),

            numberOfOtherPlayers = stream.ReadByte()
        };

        List<int> otherPlayerIds = new List<int>();

        for (int i = 0; i < cRoom.numberOfOtherPlayers; i++)
        {
            otherPlayerIds.Add(stream.ReadInt());
        }

        cRoom.otherPlayerIds = otherPlayerIds.ToArray();

        GameManager.Instance.StartCoroutine(LateInvokeRoom(cRoom));
    }

    private static IEnumerator LateInvokeRoom(Room room)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
        yield return new WaitForEndOfFrame();
        Debug.Log("Send room info");
        RoomInfoReceived?.Invoke(room);

    }

    public static void PlayerEnterRoom(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        PlayerEnterRoomReceived?.Invoke(playerId);
    }

    public static void PlayerLeaveRoom(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        PlayerLeaveRoomReceived?.Invoke(playerId);
    }

    public static void ObtainTreasure(DataStreamReader stream)
    {
        stream.ReadUInt();

        ushort amountGained = stream.ReadUShort();
        ObtainTreasureReceived?.Invoke(amountGained);
    }

    public static void HitMonster(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        ushort damageDealt = stream.ReadUShort();
        HitMonsterReceived?.Invoke(playerId, damageDealt);

    }

    public static void HitByMonster(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        ushort newCurrentHp = stream.ReadUShort();
        HitByMonsterReceived?.Invoke(playerId, newCurrentHp);
    }

    public static void PlayerDefends(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        ushort newCurrentHp = stream.ReadUShort();
        PlayerDefendsReceived?.Invoke(playerId, newCurrentHp);
    }

    public static void PlayerLeftDungeon(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        PlayerLeftDungeonReceived?.Invoke(playerId);
    }

    public static void PlayerDies(DataStreamReader stream)
    {
        stream.ReadUInt();

        int playerId = stream.ReadInt();
        PlayerDiesReceived?.Invoke(playerId);
    }

    public static void EndGame(DataStreamReader stream)
    {
        stream.ReadUInt();

        byte numberOfScores = stream.ReadByte();

        List<System.Tuple<int, ushort>> idScorePairs = new List<System.Tuple<int, ushort>>();

        for (int i = 0; i < numberOfScores; i++)
        {
            var temp = new System.Tuple<int, ushort>(stream.ReadInt(), stream.ReadUShort());

        }

        EndGameReceived?.Invoke(idScorePairs);
    }
    #endregion
}
