using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class MatchController : NetworkBehaviour
{
    public static MatchController instance;
    public SyncList<NetworkIdentity> players = new SyncList<NetworkIdentity>();
    [SyncVar] public int currentPlayer = 0;

    #region UI
    public Canvas canvas;
    #endregion

#if !UNITY_SERVER
    private void Awake()
    {
        instance = this;
        Debug.LogError("Match link!");
    }
#endif

    private void Start()
    {
        if(!isServer)
        {
            players.Callback += OnUpdatePlayers;
            canvas.worldCamera = Camera.main;
        }
    }

    private void OnUpdatePlayers(SyncList<NetworkIdentity>.Operation op, int itemIndex, NetworkIdentity oldItem, NetworkIdentity newItem)
    {
        SetupStaticLinks(newItem);
    }
    public void OnPlayerDisconnected(NetworkConnection conn)
    {

    }
    public void SetupStaticLinks(NetworkIdentity identity)
    {
        Player player = identity.GetComponent<Player>();
        if (player.isLocalPlayer) Player.localPlayer = player;
        else Player.enemyPlayer = player;
    }
}
