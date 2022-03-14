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
    public List<RectTransform> tablePoints;
    public RectTransform cardSpawnPos, cardDropPos;
    public UiInfo[] uiInfos;

    private void Awake() => instance = this;

    


    public void ChangeActiveSlots(bool value)
    {
        for (int i = 0; i < handSlots.Count; i++)
        {
            handSlots[i].ChangeActiv(value);
        }
    }

}

[System.Serializable]
public class UiInfo
{
    public TextMeshProUGUI TownHp, WallHp;
    public TextMeshProUGUI[] resourcesText;
    public TextMeshProUGUI[] addingText;
}
