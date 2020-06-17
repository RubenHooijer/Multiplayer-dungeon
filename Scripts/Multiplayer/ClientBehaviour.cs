using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class ClientBehaviour : MonoBehaviour
{
    private static uint messageid = 1;
    public static uint MessageID => messageid++;

    public NetworkDriver driver;
    public NetworkConnection connection;
    public bool Done;

    private delegate void PacketHandler(DataStreamReader stream);
    private Dictionary<uint, PacketHandler> packetHandlers;

    private void Start()
    {
        InitializePacketHandlers();

        driver = NetworkDriver.Create();
        connection = default(NetworkConnection);

        //var endPoint = NetworkEndPoint.Parse("192.168.178.20", 9000);
        //var endPoint = NetworkEndPoint.Parse("185.31.231.36", 9000);
        var endPoint = NetworkEndPoint.LoopbackIpv4;
        endPoint.Port = 9000;

        connection = driver.Connect(endPoint);

        ClientSend.Initialize(ref driver, ref connection);
    }

    private void OnDestroy()
    {
        driver.Dispose();
    }

    private void OnApplicationQuit()
    {
        driver.Disconnect(connection);
    }

    private void Update()
    {
        driver.ScheduleUpdate().Complete();

        if (!connection.IsCreated)
        {
            if (!Done)
            {
                Debug.Log("Something went wrong during connect");
            }
            return;
        }

        DataStreamReader reader;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out reader)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
            } else if (cmd == NetworkEvent.Type.Data)
            {
                packetHandlers[reader.ReadUShort()](reader);
                Done = true;
            } else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                connection = default(NetworkConnection);
                ClientHandle.OnDisconnect?.Invoke();
            }
        }
    }

    private void InitializePacketHandlers()
    {
        packetHandlers = new Dictionary<uint, PacketHandler>()
        {
            {(uint)ServerPackets.welcome, ClientHandle.Welcome },
            {(uint)ServerPackets.newPlayer, ClientHandle.NewPlayer },
            {(uint)ServerPackets.requestDenied, ClientHandle.RequestDenied },
            {(uint)ServerPackets.playerLeft, ClientHandle.PlayerLeft },
            {(uint)ServerPackets.startGame, ClientHandle.StartGame },

            {(uint)ServerPackets.playerTurn, ClientHandle.PlayerTurn },
            {(uint)ServerPackets.roomInfo, ClientHandle.RoomInfo },
            {(uint)ServerPackets.playerEnterRoom, ClientHandle.PlayerEnterRoom },
            {(uint)ServerPackets.playerLeaveRoom, ClientHandle.PlayerLeaveRoom },
            {(uint)ServerPackets.obtainTreasure, ClientHandle.ObtainTreasure },
            {(uint)ServerPackets.hitMonster, ClientHandle.HitMonster },
            {(uint)ServerPackets.hitByMonster, ClientHandle.HitByMonster },
            {(uint)ServerPackets.playerDefends, ClientHandle.PlayerDefends },
            {(uint)ServerPackets.playerLeftDungeon, ClientHandle.PlayerLeftDungeon },
            {(uint)ServerPackets.playerDies, ClientHandle.PlayerDies },
            {(uint)ServerPackets.endGame, ClientHandle.EndGame },
        };

        Debug.Log("Clientpackethandlers initialized");
    }

    public ClientBehaviour Disconnect()
    {
        connection.Disconnect(driver);
        return this;
    }
}
