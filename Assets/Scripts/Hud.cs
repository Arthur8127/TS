using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hud : MonoBehaviour
{
    public PlayerUI[] playerUIs;
    public static Hud instance;


#if !UNITY_SERVER
    private void Awake()
    {
        instance = this;
    }
#endif

    public void OnUodateUi(Player player)
    {
        int indexUi = 1;
        if (player.isLocalPlayer)
            indexUi = 0;
        playerUIs[indexUi].TownHp.text = "TownHp: " + player.TownHp;
        playerUIs[indexUi].WallHp.text = "WallHp: " + player.WallHp;

        for (int i = 0; i < playerUIs[indexUi].resources.Count; i++)
        {
            playerUIs[indexUi].resources[i].text = "Res" + i + ": " + player.Resources[i];
        }
        for (int i = 0; i < playerUIs[indexUi].adding.Count; i++)
        {
            playerUIs[indexUi].adding[i].text = "adding" + i + ": " + player.Adding[i];
        }
    }
}

[System.Serializable]
    public class PlayerUI
    {
        public bool isLocal;
        public TextMeshProUGUI WallHp, TownHp;
        public List<TextMeshProUGUI> resources = new List<TextMeshProUGUI>();
        public List<TextMeshProUGUI> adding = new List<TextMeshProUGUI>();
    }

