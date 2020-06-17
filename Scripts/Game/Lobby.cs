using Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;

    [Header("Camera positions: ")]
    [SerializeField] private Transform startCamera;
    [SerializeField] private Transform hostJoinCamera;
    [SerializeField] private Transform nameInputCamera;
    [SerializeField] private Transform lobbyCamera;

    [Header("Panels: ")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject hostJoinPanel;
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private GameObject lobbyPanel;

    [Header("Name Input: ")]
    [SerializeField] private Button nameInputButton;
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Lobby: ")]
    [SerializeField] private Button startGameButton;

    [Header("Spawn settings: ")]
    [Tooltip("0 = Wizard, Knight, Archer, Rogue")]
    [SerializeField] private Character[] charactersInOrder;
    [SerializeField] private Transform characterParent;
    [SerializeField] private Transform[] characterSpawnTransforms;

    private List<Vector3> spawnPointPositions;

    private GameObject currentPanel;
    private Coroutine currentPanelLerp;

    private void Awake()
    {
        spawnPointPositions = new List<Vector3>();
        for (int i = 0; i < characterSpawnTransforms.Length; i++)
        {
            spawnPointPositions.Add(characterSpawnTransforms[i].position);
        }
    }

    private void Start()
    {
        GoToPanel((int)LobbyPanels.Start);
    }

    private void OnEnable()
    {
        nameInputButton.onClick.AddListener(() => { SetName(nameInputField.text); nameInputField.text = string.Empty; });
    }

    private void OnDisable()
    {
        nameInputButton.onClick.RemoveListener(() => { SetName(nameInputField.text); nameInputField.text = string.Empty; });
    }

    private void OnDestroy()
    {
        ClientHandle.NewPlayerReceived -= x => RefreshLobbyPlayers();
        ClientHandle.PlayerLeftLobbyReceived -= x => RefreshLobbyPlayers();
        ClientHandle.OnDisconnect -= LeaveLobby;
    }

    public void GoToPanel(int wantedPanel)
    {
        if(currentPanelLerp != null)
        {
            StopCoroutine(currentPanelLerp);
        }

        currentPanel?.SetActive(false);
        LobbyPanels panel = (LobbyPanels)wantedPanel;

        switch (panel)
        {
            case LobbyPanels.Start:
                currentPanelLerp = StartCoroutine(PanelLerp(startCamera, startPanel));
                break;
            case LobbyPanels.HostJoin:
                currentPanelLerp = StartCoroutine(PanelLerp(hostJoinCamera, hostJoinPanel));
                break;
            case LobbyPanels.NameInput:
                currentPanelLerp = StartCoroutine(PanelLerp(nameInputCamera, nameInputPanel));
                break;
            case LobbyPanels.Lobby:
                currentPanelLerp = StartCoroutine(PanelLerp(lobbyCamera, lobbyPanel));
                break;
            default:
                currentPanelLerp = StartCoroutine(PanelLerp(startCamera, startPanel));
                break;
        }
    }

    private IEnumerator PanelLerp(Transform camTransform, GameObject panel)
    {
        panel?.SetActive(true);
        float distance = Vector3.Distance(mainCamera.position, camTransform.position);

        mainCamera.LerpRotation(camTransform.eulerAngles, 1.2f, this);
        
        yield return mainCamera.LerpPosition(camTransform.position, 1.2f, this);

        currentPanel = panel;
        currentPanelLerp = null;
    }

    public void HostGame()
    {
        ServerBehaviour s = gameObject.AddComponent<ServerBehaviour>();
        startGameButton.onClick.AddListener(ServerSend.StartGame);
        startGameButton.gameObject.SetActive(true);
        JoinGame();
    }

    public void JoinGame()
    {
        gameObject.AddComponent<GameManager>();
        gameObject.AddComponent<ClientBehaviour>();

        ClientHandle.NewPlayerReceived += x => RefreshLobbyPlayers();
        ClientHandle.PlayerLeftLobbyReceived += x => RefreshLobbyPlayers();
        ClientHandle.OnDisconnect += LeaveLobby;

        if (!gameObject.HasComponent<ServerBehaviour>())
        {
            startGameButton.gameObject.SetActive(false);
        }
    }

    public void SetName(string newName)
    {
        ClientSend.SetName(newName);

        //This looks very ugly (but it's only called once per lobby)
        GameManager.Instance.myPlayerID.name = newName;
        ClientHandle.NewPlayerReceived?.Invoke(GameManager.Instance.myPlayerID);

        GoToPanel((int)LobbyPanels.Lobby);
    }

    private void RefreshLobbyPlayers()
    {
        List<Player> players = GameManager.Instance.Players;

        characterParent.DestroyChildren();

        for (int i = 0; i < players.Count; i++)
        {
            Vector3 spawnPoint = spawnPointPositions[i];

            Character newPlayer = Instantiate(charactersInOrder[i], spawnPoint, Quaternion.identity, characterParent);
            Color32 c = new Color32();
            c = c.UIntToColor(players[i].hexColor);
            newPlayer.Initialize(c, players[i].id, $"<#{players[i].hexColor.UIntToColor()}>" + players[i].name);

        }
    }

    public void LeaveLobby()
    {
        if (gameObject.HasComponent<ServerBehaviour>())
        {
            startGameButton.onClick.RemoveListener(ServerSend.StartGame);
            Destroy(GetComponent<ServerBehaviour>());
        }
        Destroy(GetComponent<ClientBehaviour>().Disconnect());
        Destroy(GetComponent<GameManager>());

        ClientHandle.NewPlayerReceived -= x => RefreshLobbyPlayers();
        ClientHandle.PlayerLeftLobbyReceived -= x => RefreshLobbyPlayers();
        ClientHandle.OnDisconnect -= LeaveLobby;

        characterParent.DestroyChildren();
        GoToPanel((int)LobbyPanels.HostJoin);
    }
}

public enum LobbyPanels
{
    Start = 0,
    HostJoin,
    NameInput,
    Lobby
}
