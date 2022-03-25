using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Hud : MonoBehaviour
{
    public static Hud instance;
    public static Player localPlayer;
    public List<Slot> handSlots;   
    public Transform cardSpawnPos, cardDropPos, tableContent;
    public Vector2 tableOffset;
    public UiInfo[] uiInfos;

    private void Awake() => instance = this;


    



}

[System.Serializable]
public class UiInfo
{
    public TextMeshProUGUI TownHp, WallHp;
    public TextMeshProUGUI[] resourcesText;
    public TextMeshProUGUI[] addingText;
}
