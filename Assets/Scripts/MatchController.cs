using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;

public class MatchController : NetworkBehaviour
{

    public List<Player> players = new List<Player>();
    [SyncVar] public int currentPlayer = 0;
    public void OnPlayerDisconnected(NetworkConnection conn)
    {

    }
}
