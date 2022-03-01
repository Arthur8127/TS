using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hud : MonoBehaviour
{
    public PlayerUI[] playerUIs;


   
}

[System.Serializable]
public class PlayerUI
{
    public bool isLocal;
    public TextMeshProUGUI WallHp, TownHp;
    public List<TextMeshProUGUI> resources = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> adding = new List<TextMeshProUGUI>();
}

