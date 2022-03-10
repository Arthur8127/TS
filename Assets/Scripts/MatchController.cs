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
    [SyncVar(hook = nameof(OnNextPlayer))] public int currentPlayer = 0;

    public delegate void OnBlock(bool value);
    public event OnBlock onBlock;
    [SyncVar(hook = nameof(OnUpdateTimer))]
    public int TimerPlayer;



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
        CreateCardsCollection();
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
        StartCoroutine(StartPlayer());
    }
    

    public void OnNextPlayer(int oldValue, int newValue)
    {
        for (int i = 0; i < hud.dropCardContent.childCount; i++)
        {
            Destroy(hud.dropCardContent.GetChild(i).gameObject);
        }
    }

    private IEnumerator StartPlayer()
    {

        yield return new WaitForSeconds(1f);
        currentPlayer++;
        if (currentPlayer > 1) currentPlayer = 0;
        OnUpdateBlock(players[currentPlayer]);
        StartCoroutine(UpdateTimer());
    }
    private IEnumerator UpdateTimer()
    {
        
        TimerPlayer = 15;
        while (TimerPlayer > 0)
        {
            TimerPlayer--;
            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(StartPlayer());
    }

    [Command(requiresAuthority = false)]
    public void CmdDropCard(int slotID)
    {
        OnUpdateBlock(null);
        Player player = players[currentPlayer].GetComponent<Player>();
        matchAllCards.Add(GameInformation.instance.allCards[player.CardsInHands[slotID]].GetClone());
        RpcDropcard(slotID);
        int id = matchAllCards[0].index;
        matchAllCards.RemoveAt(0);
        player.CardsInHands[slotID] = id;
        StopAllCoroutines();
        StartCoroutine(StartPlayer());
    }

    [ClientRpc]
    private void RpcDropcard(int slotID)
    {
        UiCardPrefab card = Instantiate(hud.UICardPrefab, hud.dropCardContent).GetComponent<UiCardPrefab>();
        card.img.sprite = hud.cardBgSprite;
        card.img.color = Color.white;
        card.transform.position = hud.slots[slotID].position;
        card.StartCoroutine(card.MoveZirroPos());
    }

    [ClientRpc]
    public void OnUpdateBlock(NetworkIdentity identity)
    {
        bool value;
        if (identity == null) value = false;
        else value = identity.isLocalPlayer;

        onBlock?.Invoke(value);
        hud.playerUIs[0].timerText.text = "";
        hud.playerUIs[1].timerText.text = "";
    }
    private void OnUpdateTimer(int oldValue, int newValue)
    {
        hud.UpdateTimer(newValue);
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
