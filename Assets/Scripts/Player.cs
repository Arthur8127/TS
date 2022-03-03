using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    
    [SyncVar(hook = nameof(OnUpdateWallHp))]
    public int WallHp;
    [SyncVar(hook = nameof(OnUpdateTownHp))]
    public int TownHp;
    public SyncList<int> Resources = new SyncList<int>();
    public SyncList<int> Adding = new SyncList<int>();

    private void Start()
    {
        if (!isServer)
        {
            Debug.LogError("start!");
            Resources.Callback += Resources_Callback;
            Adding.Callback += Adding_Callback;
            
        }
    }

    
    
    #region CallBacks SyncVars
    private void OnUpdateTownHp(int old, int newValue)
    {
        if (MatchController.instance == null) return;
        MatchController.instance.hud.OnUodateUi(this);
    }
    private void OnUpdateWallHp(int old, int newValue)
    {
        if (MatchController.instance == null) return;
        MatchController.instance.hud.OnUodateUi(this);
    }
    private void Resources_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        if (MatchController.instance == null) return;
        MatchController.instance.hud.OnUodateUi(this);
    }
    private void Adding_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        if (MatchController.instance == null) return;
        MatchController.instance.hud.OnUodateUi(this);
    }
    #endregion
}

