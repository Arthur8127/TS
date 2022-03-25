using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using Random = UnityEngine.Random;

public class MatchController : NetworkBehaviour
{
    public static MatchController instance;
    public Hud hud;
    public List<CardBase> cardsMatch = new List<CardBase>();
    public List<Player> players = new List<Player>();
    private int playerIndex = 0;
    [SyncVar(hook =nameof(OnUpdateCurrentPlayer))]
    public NetworkIdentity currentPlayer;



    public void Setup()
    {
        currentPlayer = players[0].netIdentity;
        cardsMatch = GameInformation.instance.GetCards();
        foreach (var p in players)
        {
            for (int i = 0; i < 6; i++)
            {
                p.cards.Add(cardsMatch[0].index);
                cardsMatch.RemoveAt(0);
            }
            for (int i = 0; i < 3; i++)
            {
                p.resources.Add(10);
                p.adding.Add(5);
            }
            p.WallHp = 20;
            p.TownHp = 30;
        }
        
    }

    public void StartMatch()
    {
        Debug.LogError("START MATCH!");
    }

    public void OnUpdateCurrentPlayer(NetworkIdentity _, NetworkIdentity newValue)
    {
        for (int i = 0; i < Hud.instance.handSlots.Count; i++)
        {
            Hud.instance.handSlots[i].SetActive(newValue.isLocalPlayer);
        }
    }
    public void OnPlayerDisconnected(NetworkConnection conn)
    {

    }
}