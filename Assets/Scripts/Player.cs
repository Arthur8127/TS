using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;
    public static Player enemyPlayer;
    [SyncVar(hook =nameof(OnUpdateWallHp))]
    public int WallHp;
    [SyncVar(hook = nameof(OnUpdateTownHp))]
    public int TownHp;
    public SyncList<int> Resources = new SyncList<int>();
    public SyncList<int> Adding = new SyncList<int>();

    private void Start()
    {
        if (!isServer)
        {
            Resources.Callback += Resources_Callback;
            Adding.Callback += Adding_Callback;            
        }
    }

    
    #region CallBacks SyncVars
    private void OnUpdateTownHp(int old, int newValue)
    {

    }
    private void OnUpdateWallHp(int old, int newValue)
    {

    }
    private void Resources_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {

    }
    private void Adding_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {

    }
    #endregion
}

