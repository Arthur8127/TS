using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image cardUi;
    public CanvasGroup canvasGroup;
    public int Index;
    public int cardId = -1;
    public Vector2[] slotSize;
    public bool isActive = false;
    public Color noActiveColor;
    public Vector2 noActivePos;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // пытаемся сходить картой в этом слоте
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Hud.instance.ChangeActiveSlots(false);
            Hud.localPlayer.CmdDropCard(Index, cardId);
            canvasGroup.alpha = 0;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActive) return;
        transform.SetAsLastSibling();
        (cardUi.transform as RectTransform).sizeDelta = slotSize[1];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isActive) return;
        transform.SetSiblingIndex(Index);
        (cardUi.transform as RectTransform).sizeDelta = slotSize[0];
    }

    public void AddCard(int id)
    {
        canvasGroup.alpha = 1;
        cardId = id;
        cardUi.sprite = GameInformation.instance.allCards[id].sprite;
        cardUi.transform.position = Hud.instance.cardSpawnPos.position;
        StartCoroutine(Move());
        ChangeActiv(false);
    }

    private IEnumerator Move()
    {
        while (cardUi.transform.localPosition != Vector3.zero)
        {
            yield return null;
            cardUi.transform.localPosition = Vector3.MoveTowards(cardUi.transform.localPosition, Vector3.zero, 50f);
        }

    }

   

    public void ChangeActiv(bool value)
    {
        if(value)
        {
            isActive = true;
            cardUi.color = Color.white;
            CheckResources();
        }
        else
        {
            isActive = false;
            cardUi.color = noActiveColor;          
        }
        
    }
   
    private void CheckResources()
    {
        int resID = (int)GameInformation.instance.allCards[cardId].nedeedResourcesType;
        int resCount = GameInformation.instance.allCards[cardId].nedeedCount;
        if (Hud.localPlayer.resources[resID] < resCount)
        {
            isActive = false;
            cardUi.transform.localPosition = noActivePos;
        }
       
    }
}
