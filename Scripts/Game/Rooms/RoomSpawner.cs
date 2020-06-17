using EasyAttributes;
using Extensions;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room settings: ")]
    [SerializeField] private PhysicalRoom[] rooms;
    [SerializeField] private Camera mainCamera;

    [Header("Characters")]
    [SerializeField] private GameObject[] characters;

    private PhysicalRoom currentRoom;
    private GameObject currentRoomPrefab;
    private List<GameObject> currentCharacters = new List<GameObject>();

    public Room fooRoom;

    private void Awake()
    {
        ClientHandle.RoomInfoReceived += HandleRoom;
    }

    [Button]
    private void FooRoom()
    {
        HandleRoom(fooRoom);
    }

    private void HandleRoom(Room room)
    {
        SpawnRoom(room.possibleDirections);
        DestroyCharacters();
        SpawnCharacters(GameManager.Instance.myPlayerID.id);
        if(room.numberOfOtherPlayers > 0)
        {
            SpawnCharacters(room.otherPlayerIds);
        }
    }

    private void SpawnRoom(Direction dir)
    {
        if(currentRoomPrefab != null)
        {
            Destroy(currentRoomPrefab);
        }

        currentRoom = GetRandomRoom(dir);
        
        mainCamera.transform.position = currentRoom.cameraPosition;
        mainCamera.transform.eulerAngles = currentRoom.cameraRotation;

        currentRoomPrefab = Instantiate(currentRoom.gameObject);

        Compass.ChangeDirection(currentRoom.whatIsNorth);
    }

    private void SpawnCharacters(params int[] otherPlayerIds)
    {
        for (int i = 0; i < otherPlayerIds.Length; i++)
        {
            var p = GameManager.Instance.GetPlayerWithId(otherPlayerIds[i]);
            var c = currentRoom.SpawnClass(characters[(int)p.playerClass], p.playerClass);
            c.GetComponent<Character>().SetName($"<#{p.hexColor.UIntToColor()}>" + p.name);

            currentCharacters.Add(c);
        }
    }

    private void DestroyCharacters()
    {
        if (currentCharacters.Count < 1) return;

        for (int i = 0; i < currentCharacters.Count; i++)
        {
            Destroy(currentCharacters[i]);
        }

        currentCharacters.Clear();
    }

    private PhysicalRoom GetRandomRoom(Direction dir)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            if(rooms[i].availableDirections == dir)
            {
                return rooms[i];
            }
        }

        return rooms[0];
    }
}
