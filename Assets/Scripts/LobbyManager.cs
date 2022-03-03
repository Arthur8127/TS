using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    public event Action<NetworkConnection> OnPlayerDisconnected;
    internal static readonly Dictionary<NetworkConnection, Guid> playerMatches = new Dictionary<NetworkConnection, Guid>();
    internal static readonly Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();
    internal static readonly Dictionary<Guid, HashSet<NetworkConnection>> matchConnections = new Dictionary<Guid, HashSet<NetworkConnection>>();
    internal static readonly Dictionary<NetworkConnection, PlayerInfo> playerInfos = new Dictionary<NetworkConnection, PlayerInfo>();
    internal static readonly List<NetworkConnection> waitingConnections = new List<NetworkConnection>();
    internal static List<MatchData> allMatches = new List<MatchData>();
    internal Guid localPlayerMatch = Guid.Empty;
    internal Guid localJoinedMatch = Guid.Empty;
    internal Guid selectedMatch = Guid.Empty;
    int playerIndex = 1;
    private bool isOwner;
    public GameObject matchControllerPrefab;
    public Button btnGame;
    public Text btnText;
    public GameObject lobbyCanvas;
    public PlayerData playerData;
    
    private void Awake()
    {
        instance = this;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetStatics()
    {
        playerMatches.Clear();
        openMatches.Clear();
        matchConnections.Clear();
        playerInfos.Clear();
        waitingConnections.Clear();
    }
    internal void InitializeData()
    {
        playerMatches.Clear();
        openMatches.Clear();
        matchConnections.Clear();
        waitingConnections.Clear();
        localPlayerMatch = Guid.Empty;
        localJoinedMatch = Guid.Empty;
    }
    void ResetCanvas()
    {
        InitializeData();
    }


    #region Button Calls

    public void SelectMatch(Guid matchId)
    {
        if (!NetworkClient.active) return;

        if (matchId == Guid.Empty)
        {
            selectedMatch = Guid.Empty;
        }
        else
        {
            if (!openMatches.Keys.Contains(matchId)) return;
            selectedMatch = matchId;
            MatchInfo infos = openMatches[matchId];
        }
    }
    public void RequestCreateMatch()
    {
        if (!NetworkClient.active) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Create });
    }

    public void RequestJoinMatch()
    {
        if (!NetworkClient.active || selectedMatch == Guid.Empty) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Join, matchId = selectedMatch });
    }
    public void RequestLeaveMatch()
    {
        if (!NetworkClient.active || localJoinedMatch == Guid.Empty) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, matchId = localJoinedMatch });
    }
    public void RequestCancelMatch()
    {
        if (!NetworkClient.active || localPlayerMatch == Guid.Empty) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Cancel });
    }
    public void RequestReadyChange()
    {
        if (!NetworkClient.active || (localPlayerMatch == Guid.Empty && localJoinedMatch == Guid.Empty)) return;

        Guid matchId = localPlayerMatch == Guid.Empty ? localJoinedMatch : localPlayerMatch;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Ready, matchId = matchId });
    }
    public void RequestStartMatch()
    {
        if (!NetworkClient.active || localPlayerMatch == Guid.Empty) return;

        NetworkClient.connection.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Start });
    }
    public void OnMatchEnded()
    {
        if (!NetworkClient.active) return;

        localPlayerMatch = Guid.Empty;
        localJoinedMatch = Guid.Empty;
        ShowLobbyView();
    }
    internal void SendMatchList(NetworkConnection conn = null)
    {
        if (!NetworkServer.active) return;

        if (conn != null)
        {
            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
        }
        else
        {
            foreach (var waiter in waitingConnections)
            {
                waiter.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
            }
        }
    }

    #endregion

    #region Server & Client Callbacks
    internal void OnStartServer()
    {
        if (!NetworkServer.active) return;

        InitializeData();
        NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
    }
    internal void OnServerReady(NetworkConnection conn)
    {
        if (!NetworkServer.active) return;

        waitingConnections.Add(conn);
        playerInfos.Add(conn, new PlayerInfo { playerIndex = this.playerIndex, ready = false });
        playerIndex++;

        SendMatchList();
    }
    internal void OnServerDisconnect(NetworkConnection conn)
    {
        if (!NetworkServer.active) return;

        // Invoke OnPlayerDisconnected on all instances of MatchController
        OnPlayerDisconnected?.Invoke(conn);

        Guid matchId;
        if (playerMatches.TryGetValue(conn, out matchId))
        {
            playerMatches.Remove(conn);
            openMatches.Remove(matchId);

            foreach (NetworkConnection playerConn in matchConnections[matchId])
            {
                PlayerInfo _playerInfo = playerInfos[playerConn];
                _playerInfo.ready = false;
                _playerInfo.matchId = Guid.Empty;
                playerInfos[playerConn] = _playerInfo;
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
            }
        }

        foreach (KeyValuePair<Guid, HashSet<NetworkConnection>> kvp in matchConnections)
        {
            kvp.Value.Remove(conn);
        }

        PlayerInfo playerInfo = playerInfos[conn];
        if (playerInfo.matchId != Guid.Empty)
        {
            MatchInfo matchInfo;
            if (openMatches.TryGetValue(playerInfo.matchId, out matchInfo))
            {
                matchInfo.players--;
                openMatches[playerInfo.matchId] = matchInfo;
            }

            HashSet<NetworkConnection> connections;
            if (matchConnections.TryGetValue(playerInfo.matchId, out connections))
            {
                PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

                foreach (NetworkConnection playerConn in matchConnections[playerInfo.matchId])
                {
                    if (playerConn != conn)
                    {
                        playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
                    }
                }
            }
        }

        SendMatchList();
    }
    internal void OnStopServer()
    {
        ResetCanvas();
    }
    internal void OnClientConnect()
    {
        playerInfos.Add(NetworkClient.connection, new PlayerInfo { playerIndex = this.playerIndex, ready = false });
    }
    internal void OnStartClient()
    {
        if (!NetworkClient.active) return;
        InitializeData();
        ShowLobbyView();
        NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
    }
    internal void OnClientDisconnect()
    {
        if (!NetworkClient.active) return;

        InitializeData();
    }
    internal void OnStopClient()
    {
        ResetCanvas();
    }
    #endregion

    #region Server Match Message Handlers
    void OnServerMatchMessage(NetworkConnection conn, ServerMatchMessage msg)
    {
        if (!NetworkServer.active) return;

        switch (msg.serverMatchOperation)
        {
            case ServerMatchOperation.None:
                {
                    Debug.LogWarning("Missing ServerMatchOperation");
                    break;
                }
            case ServerMatchOperation.Create:
                {
                    OnServerCreateMatch(conn);
                    break;
                }
            case ServerMatchOperation.Cancel:
                {
                    OnServerCancelMatch(conn);
                    break;
                }
            case ServerMatchOperation.Start:
                {
                    OnServerStartMatch(conn);
                    break;
                }
            case ServerMatchOperation.Join:
                {
                    OnServerJoinMatch(conn, msg.matchId);
                    break;
                }
            case ServerMatchOperation.Leave:
                {
                    OnServerLeaveMatch(conn, msg.matchId);
                    break;
                }
            case ServerMatchOperation.Ready:
                {
                    OnServerPlayerReady(conn, msg.matchId);
                    break;
                }
        }
    }
    void OnServerPlayerReady(NetworkConnection conn, Guid matchId)
    {
        if (!NetworkServer.active) return;

        PlayerInfo playerInfo = playerInfos[conn];
        playerInfo.ready = !playerInfo.ready;
        playerInfos[conn] = playerInfo;

        HashSet<NetworkConnection> connections = matchConnections[matchId];
        PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

        foreach (NetworkConnection playerConn in matchConnections[matchId])
        {
            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }
    }
    void OnServerLeaveMatch(NetworkConnection conn, Guid matchId)
    {
        if (!NetworkServer.active) return;

        MatchInfo matchInfo = openMatches[matchId];
        matchInfo.players--;
        openMatches[matchId] = matchInfo;

        PlayerInfo playerInfo = playerInfos[conn];
        playerInfo.ready = false;
        playerInfo.matchId = Guid.Empty;
        playerInfos[conn] = playerInfo;

        foreach (KeyValuePair<Guid, HashSet<NetworkConnection>> kvp in matchConnections)
        {
            kvp.Value.Remove(conn);
        }

        HashSet<NetworkConnection> connections = matchConnections[matchId];
        PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

        foreach (NetworkConnection playerConn in matchConnections[matchId])
        {
            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }

        SendMatchList();

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
    }
    void OnServerCreateMatch(NetworkConnection conn)
    {
        if (!NetworkServer.active || playerMatches.ContainsKey(conn)) return;

        Guid newMatchId = Guid.NewGuid();
        matchConnections.Add(newMatchId, new HashSet<NetworkConnection>());
        matchConnections[newMatchId].Add(conn);
        playerMatches.Add(conn, newMatchId);
        openMatches.Add(newMatchId, new MatchInfo { matchId = newMatchId, maxPlayers = 2, players = 1 });

        PlayerInfo playerInfo = playerInfos[conn];
        playerInfo.ready = true;
        playerInfo.matchId = newMatchId;
        playerInfos[conn] = playerInfo;

        PlayerInfo[] infos = matchConnections[newMatchId].Select(playerConn => playerInfos[playerConn]).ToArray();

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Created, matchId = newMatchId, playerInfos = infos });

        SendMatchList();
    }
    void OnServerCancelMatch(NetworkConnection conn)
    {
        if (!NetworkServer.active || !playerMatches.ContainsKey(conn)) return;

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Cancelled });

        Guid matchId;
        if (playerMatches.TryGetValue(conn, out matchId))
        {
            playerMatches.Remove(conn);
            openMatches.Remove(matchId);

            foreach (NetworkConnection playerConn in matchConnections[matchId])
            {
                PlayerInfo playerInfo = playerInfos[playerConn];
                playerInfo.ready = false;
                playerInfo.matchId = Guid.Empty;
                playerInfos[playerConn] = playerInfo;
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
            }

            SendMatchList();
        }
    }
    void OnServerStartMatch(NetworkConnection conn)
    {
        if (!NetworkServer.active || !playerMatches.ContainsKey(conn)) return;

        Guid matchId;
        if (playerMatches.TryGetValue(conn, out matchId))
        {
            GameObject matchControllerObject = Instantiate(matchControllerPrefab);
            matchControllerObject.GetComponent<NetworkMatch>().matchId = matchId;
            NetworkServer.Spawn(matchControllerObject);

            MatchController matchController = matchControllerObject.GetComponent<MatchController>();
          
            MatchData matchData = new MatchData();
            matchData.matchID = matchId;
            matchData.matchController = matchController;
            allMatches.Add(matchData);
            
            foreach (NetworkConnection playerConn in matchConnections[matchId])
            {
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Started });

                GameObject player = Instantiate(NetManager.instance.playerPrefab);
                player.GetComponent<NetworkMatch>().matchId = matchId;

                NetworkServer.AddPlayerForConnection(playerConn, player);

                matchController.players.Add(playerConn.identity);
                PlayerInfo playerInfo = playerInfos[playerConn];
                playerInfo.ready = true;
                playerInfos[playerConn] = playerInfo;
                
                if(matchController.players.Count == 2)
                {
                    matchController.SetupPlayer();
                }
                
            }


            playerMatches.Remove(conn);
            openMatches.Remove(matchId);
            matchConnections.Remove(matchId);
            SendMatchList();
            OnPlayerDisconnected += matchController.OnPlayerDisconnected;

        }
    }

    

    void OnServerJoinMatch(NetworkConnection conn, Guid matchId)
    {
        if (!NetworkServer.active || !matchConnections.ContainsKey(matchId) || !openMatches.ContainsKey(matchId)) return;

        MatchInfo matchInfo = openMatches[matchId];
        matchInfo.players++;
        openMatches[matchId] = matchInfo;
        matchConnections[matchId].Add(conn);

        PlayerInfo playerInfo = playerInfos[conn];
        playerInfo.ready = true;
        playerInfo.matchId = matchId;
        playerInfos[conn] = playerInfo;

        PlayerInfo[] infos = matchConnections[matchId].Select(playerConn => playerInfos[playerConn]).ToArray();
        SendMatchList();

        conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Joined, matchId = matchId, playerInfos = infos });

        foreach (NetworkConnection playerConn in matchConnections[matchId])
        {
            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }
    }
#endregion

    #region Client Match Message Handler

    void OnClientMatchMessage(ClientMatchMessage msg)
    {
        if (!NetworkClient.active) return;

        switch (msg.clientMatchOperation)
        {
            case ClientMatchOperation.None:
                {
                    Debug.LogWarning("Missing ClientMatchOperation");
                    break;
                }
            case ClientMatchOperation.List:
                {
                    openMatches.Clear();
                    foreach (MatchInfo matchInfo in msg.matchInfos)
                    {
                        openMatches.Add(matchInfo.matchId, matchInfo);
                    }
                    RefreshMatchList();
                    break;
                }
            case ClientMatchOperation.Created:
                {
                    localPlayerMatch = msg.matchId;
                    ShowRoomView();
                    Debug.LogError("Ты создал маеч");
                    isOwner = true;
                    break;
                }
            case ClientMatchOperation.Cancelled:
                {
                    localPlayerMatch = Guid.Empty;
                    ShowLobbyView();
                    break;
                }
            case ClientMatchOperation.Joined:
                {
                    localJoinedMatch = msg.matchId;
                    ShowRoomView();
                    Debug.LogError("Ты подключился к матчу");
                    break;
                }
            case ClientMatchOperation.Departed:
                {
                    localJoinedMatch = Guid.Empty;
                    ShowLobbyView();
                    break;
                }
            case ClientMatchOperation.UpdateRoom:
                {
                    StartCoroutine(StartMatchCar());
                    break;
                }
            case ClientMatchOperation.Started:
                {
                    break;
                }
        }
    }
    void ShowLobbyView()
    {
        lobbyCanvas.SetActive(true);
        btnGame.interactable = true;
        btnText.text = "JoinOrCreateRoom";
    }
    void ShowRoomView()
    {

    }
    void RefreshMatchList()
    {

    }

    public void JoinOrCreateMatch()
    {
        foreach (var item in openMatches)
        {
            if (item.Value.players < item.Value.maxPlayers)
            {
                SelectMatch(item.Value.matchId);
                RequestJoinMatch();
                return;
            }
        }
        RequestCreateMatch();
    }

    private IEnumerator StartMatchCar()
    {
        int time = 5;
        btnGame.interactable = false;
        while (time > 0)
        {
            time--;
            btnText.text = "Starting: " + time;
            yield return new WaitForSeconds(1f);
        }
        if (isOwner)
        {
            RequestStartMatch();
            isOwner = false;
        }
        lobbyCanvas.SetActive(false);
    }
    #endregion
}

[System.Serializable]
public class PlayerData
{
    public int WallHp;
    public int TownHp;
    public int[] Resources;
    public int[] Adding;
}

public class MatchData
{
    public Guid matchID;
    public MatchController matchController;
}
