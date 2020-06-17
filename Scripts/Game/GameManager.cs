using EasyAttributes;
using Extensions.Generics.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GenericSingleton<GameManager, GameManager>
{
    public List<Player> Players /*{ get; private set; }*/ = new List<Player>();
    public Player myPlayerID;

    protected override void Awake()
    {
        DontDestroyOnLoad(this);

        ClientHandle.WelcomeReceived += SetMyPlayer;
        ClientHandle.NewPlayerReceived += AddPlayer;
        ClientHandle.PlayerLeftLobbyReceived += RemovePlayer;
        ClientHandle.StartGameReceived += OnStartGame;
    }

    [Button]
    private void FooAddMyPlayerID()
    {
        myPlayerID = new Player()
        {
            id = 0,
            name = "harry",
            hexColor = 3000,
            playerClass = PlayerClasses.Wizard
        };

        AddPlayer(myPlayerID);
    }

    public Player GetPlayerWithId(int id)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if(Players[i].id == id)
            {
                return Players[i];
            }
        }

        return Players[0];
    }

    private void SetMyPlayer(Player player)
    {
        myPlayerID = player;
    }

    private void AddPlayer(Player player)
    {
        Players.Add(player);
        Players = Players.OrderBy(p => p.id).ToList();

        int pIndex = Players.IndexOf(player);
        player.playerClass = (PlayerClasses)pIndex;
        Players[pIndex] = player;
    }

    private void RemovePlayer(int id)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].id != id) continue;

            Players.RemoveAt(i);
            return;
        }
    }

    private void OnStartGame()
    {
        Destroy(GetComponent<Lobby>());
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void OnLevelWasLoaded(int level)
    {

        switch (level)
        {
            //Lobby
            case 0:
                Destroy(gameObject);
                return;
            //Game
            case 1:
                return;
            //EndScene
            case 2:
                return;
            default:
                return;
        }
    }
}
