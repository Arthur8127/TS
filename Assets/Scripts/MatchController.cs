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
    public int currentPlayer = 0;


    public void Setup()
    {
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
        players[currentPlayer].RpcActivate();
    }

    public void OnPlayerDisconnected(NetworkConnection conn)
    {

    }
}