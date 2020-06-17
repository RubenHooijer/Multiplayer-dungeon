using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using Extensions.Generics.Singleton;
using Extensions;

public class ServerBehaviour : GenericSingleton<ServerBehaviour, ServerBehaviour>
{
    private static uint messageid = 1;
    public static uint MessageID => messageid++;

    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private delegate void PacketHandler(DataStreamReader stream, NetworkConnection connection);
    private Dictionary<uint, PacketHandler> packetHandlers;

    public List<Player> players = new List<Player>();
    public string[] availableHexColors = new string[] {/*blue*/"2e7eff", /*yellow*/"ebd917", /*green*/"17eb53", /*purple*/"8206cf" };
    public Room[,] dungeon;

    private void Start()
    {
        InitializePacketHandlers();

        driver = NetworkDriver.Create();
        var endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = 9000;

        if (driver.Bind(endPoint) != 0)
        {
            Debug.Log($"Failed to bind to port {endPoint}");
        } else
        {
            driver.Listen();
        }

        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        ServerSend.Initialize(ref driver, ref connections);
    }

    public int GetPlayerWithId(int id)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id == id)
            {
                return i;
            }
        }

        return 0;
    }

    private void OnDestroy()
    {
        driver.Dispose();
        connections.Dispose();
    }

    private void Update()
    {
        driver.ScheduleUpdate().Complete();

        //Clean up old connections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i; //Go back because we removed an index
            }
        }

        //Accept new connection - returns an up-to-date connection list
        NetworkConnection connection;
        while ((connection = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(connection);
            Debug.Log("Accepted a connection");

            ServerSend.Welcome(connection, connection.InternalId, availableHexColors[connection.InternalId].ColorToUInt());
        }


        //Start querying driver events that might have happened since the last update
        DataStreamReader reader;
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                continue;
            }
            
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out reader)) != NetworkEvent.Type.Empty)
            {
                //Process events
                if (cmd == NetworkEvent.Type.Data)
                {
                    packetHandlers[reader.ReadUShort()](reader, connections[i]);

                    //Handle disconnections
                } else if(cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    ServerSend.PlayerLeft(connections[i].InternalId);
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    private void InitializePacketHandlers()
    {
        packetHandlers = new Dictionary<uint, PacketHandler>()
        {
            {(uint)ClientPackets.setName, ServerHandle.SetName },

            {(uint)ClientPackets.moveRequest, ServerHandle.MoveRequest },
            {(uint)ClientPackets.attackRequest, ServerHandle.AttackRequest },
            {(uint)ClientPackets.defendRequest, ServerHandle.DefendRequest },
            {(uint)ClientPackets.claimTreasureRequest, ServerHandle.ClaimTreasureRequest },
            {(uint)ClientPackets.leaveDungeonRequest, ServerHandle.LeaveDungeonRequest }
        };

        Debug.Log("Serverpackethandlers initialized");
    }
}
