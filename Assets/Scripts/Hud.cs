using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Hud : MonoBehaviour
{
    [HideInInspector]public Player localPlayer;
    public PlayerUI[] playerUIs;
    public List<RectTransform> slots;
    public GameObject UICardPrefab;
    public RectTransform cardSpawnPos, dropCardContent;
    public Sprite cardBgSprite;


    public void OnUodateUi(Player player)
    {
        int indexUi;

        if (player.isLocalPlayer)
        {

            indexUi = 0;
        }
        else
        {

            indexUi = 1;
        }

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

    public void AddCardInSlot(int slotIndex, int cardID)
    {
        UiCardPrefab uiCard = Instantiate(UICardPrefab, slots[slotIndex]).GetComponent<UiCardPrefab>();
        uiCard.cardID = cardID;
        uiCard.img.sprite = GameInformation.instance.allCards[cardID].sprite;
        uiCard.transform.position = cardSpawnPos.position;
        uiCard.transform.parent.SetSiblingIndex(slotIndex);
        uiCard.StartCoroutine(uiCard.MoveZirroPos());
    }

    public void UpdateTimer(int value)
    {
        TextMeshProUGUI text;
        if (localPlayer.IsCurrentPlayer) text = playerUIs[0].timerText;
        else text = playerUIs[1].timerText;
        text.text = "Timer:" + value;
    }
}

[System.Serializable]
public class PlayerUI
{
    public bool isLocal;
    public TextMeshProUGUI WallHp, TownHp;
    public List<TextMeshProUGUI> resources = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> adding = new List<TextMeshProUGUI>();
    public TextMeshProUGUI timerText;
}

