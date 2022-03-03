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
        else
        {
            TownHp = 20;
            WallHp = 20;
            for (int i = 0; i < 3; i++)
            {
                Resources.Add(10);
            }
            for (int i = 0; i < 3; i++)
            {
                Adding.Add(3);
            }
        }
    }

    
    #region CallBacks SyncVars
    private void OnUpdateTownHp(int old, int newValue)
    {
        Hud.instance.OnUodateUi(this);
    }
    private void OnUpdateWallHp(int old, int newValue)
    {
        Hud.instance.OnUodateUi(this);
    }
    private void Resources_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        Hud.instance.OnUodateUi(this);
    }
    private void Adding_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        Hud.instance.OnUodateUi(this);
    }
    #endregion
}

