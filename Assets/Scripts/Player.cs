using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using System.Linq;

public class Player : NetworkBehaviour
{

    public GameObject cardPrefab;
    public SyncList<int> cards = new SyncList<int>();
    [SyncVar(hook = nameof(OnUpdateWallHp))] public int WallHp;
    [SyncVar(hook = nameof(OnUpdateTownHp))] public int TownHp;



    public SyncList<int> resources = new SyncList<int>();
    public SyncList<int> adding = new SyncList<int>();

    private void Start()
    {
        if (isClient)
        {
            resources.Callback += OnUpdateResources;
            adding.Callback += OnUpdateAdding;
        }
    }

    public override void OnStartAuthority()
    {
        MatchController.instance = FindObjectOfType<MatchController>();
        Hud.instance = FindObjectOfType<Hud>();
        Hud.localPlayer = this;
        cards.Callback += OnUpdateCards;
    }

    private void OnUpdateCards(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {


        switch (op)
        {
            case SyncList<int>.Operation.OP_ADD:
                Hud.instance.handSlots[itemIndex].AddCard(newItem);
                break;
            case SyncList<int>.Operation.OP_CLEAR:
                break;
            case SyncList<int>.Operation.OP_INSERT:
                break;
            case SyncList<int>.Operation.OP_REMOVEAT:
                break;
            case SyncList<int>.Operation.OP_SET:
                break;

        }
    }

    private void OnUpdateAdding(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        int index = 1;
        if (isLocalPlayer) index = 0;
        if (adding.Count < 3) return;
        Hud.instance.uiInfos[index].addingText[0].text = "Dangens: " + adding[0];
        Hud.instance.uiInfos[index].addingText[1].text = "Magic: " + adding[1];
        Hud.instance.uiInfos[index].addingText[2].text = "Dangen: " + adding[2];


    }

    private void OnUpdateResources(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        int index = 1;
        if (isLocalPlayer) index = 0;
        if (resources.Count < 3) return;
        Hud.instance.uiInfos[index].resourcesText[0].text = "Recruts: " + resources[0];
        Hud.instance.uiInfos[index].resourcesText[1].text = "Gems: " + resources[1];
        Hud.instance.uiInfos[index].resourcesText[2].text = "Bricks: " + resources[2];
        //recruts = dangens
        //gems = magic
        // bricks = quary
    }
    private void OnUpdateWallHp(int oldValue, int newValue)
    {
        int index = 1;
        if (isLocalPlayer) index = 0;
        Hud.instance.uiInfos[index].WallHp.text = "Wall " + newValue;
    }
    private void OnUpdateTownHp(int oldValue, int newValue)
    {
        int index = 1;
        if (isLocalPlayer) index = 0;
        Hud.instance.uiInfos[index].TownHp.text = "Towen " + newValue;
    }

    [TargetRpc]
    public void RpcActivate()
    {
        Hud.instance.ChangeActiveSlots(true);
    }
    [Command]
    public void CmdDropCard(int slotID, int cardID)
    {
        var matchID = GetComponent<NetworkMatch>().matchId;
        MatchController match = LobbyManager.allMatches.Where(x => x.matchID == matchID).FirstOrDefault().matchController;
      
        GameObject obj = Instantiate(cardPrefab, match.hud.cardDropPos);
        obj.GetComponent<NetworkMatch>().matchId = matchID;
        obj.transform.position = match.hud.handSlots[slotID].transform.position;
        CardUiPrefab card = obj.GetComponent<CardUiPrefab>();
        NetworkServer.Spawn(obj);
        card.StartCoroutine(card.MoveInParent());
    }
}



