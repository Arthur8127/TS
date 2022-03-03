using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class MatchController : NetworkBehaviour
{
    public static MatchController instance;
    public Hud hud;
    public List<NetworkIdentity> players = new List<NetworkIdentity>();
    [SyncVar] public int currentPlayer = 0;

    #region UI
    public Canvas canvas;
    #endregion

#if !UNITY_SERVER
    private void Awake()
    {
        Debug.LogError("Match Awake!");
        instance = this;
    }
#endif

    private void Start()
    {
        if (!isServer)
        {
            canvas.worldCamera = Camera.main;
        }

    }

    public void OnPlayerDisconnected(NetworkConnection conn)
    {

    }



    public void SetupPlayer()
    {
        for (int x = 0; x < players.Count; x++)
        {
            Player player = players[x].GetComponent<Player>();
            player.WallHp = LobbyManager.instance.playerData.WallHp;
            player.TownHp = LobbyManager.instance.playerData.TownHp;
            for (int i = 0; i < 3; i++)
            {
                player.Resources.Add(LobbyManager.instance.playerData.Resources[i]);
                player.Adding.Add(LobbyManager.instance.playerData.Adding[i]);
            }
        }
    }

}
