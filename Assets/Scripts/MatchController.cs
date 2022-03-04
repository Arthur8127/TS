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
    public List<CardBase> matchAllCards = new List<CardBase>();
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
        else
        {
            CreateCardsCollection();
        }

    }

    public void OnPlayerDisconnected(NetworkConnection conn)
    {

    }

    private void CreateCardsCollection()
    {
        for (int i = 0; i < GameInformation.instance.allCards.Count; i++) matchAllCards.Add(GameInformation.instance.allCards[i].GetClone());

        for (int i = 0; i < matchAllCards.Count; i++)
        {
            CardBase currentCard = matchAllCards[i];
            int randomIndex = Random.Range(i, matchAllCards.Count);
            matchAllCards[i] = matchAllCards[randomIndex];
            matchAllCards[randomIndex] = currentCard;
        }
        
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
            for (int i = 0; i < 6; i++)
            {
                player.CardsInHands.Add(matchAllCards[0].index);
                matchAllCards.RemoveAt(0);
            }
        }

    }
    #region Actions
    public void TownVitals(MessageParam param)
    {

    }
    public void WallVitals(MessageParam param)
    {

    }
    public void Damage(MessageParam param)
    {

    }
    public void ResourcesEdit(MessageParam param)
    {

    }
    public void AddingEdit(MessageParam param)
    {

    }
    public void PlayAgain(MessageParam param)
    {

    }
    public void SwichWalls(MessageParam param)
    {

    }
    public void Discard(MessageParam param)
    {

    }
    public void EqualizeQuarry(MessageParam param)
    {
        // игрок у которого ниже стена получает
        // -1 duration & 2 damage Town
    }
    public void MagicEqualsHighestPlayer(MessageParam param)
    {

    }
    #endregion
}
