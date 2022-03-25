using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image sprite;    
    public int slotID;
    public int cardId = -1;
    public bool isActive = false;
    public Color noActiveColor;
    public Vector2 noActivePos;
    public Vector2 selectSize;
    public Vector2 normalSize;

   

    public void AddCard(int id)
    {        
        cardId = id;
        sprite.sprite = GameInformation.instance.allCards[id].sprite;
        sprite.transform.position = Hud.instance.cardSpawnPos.position;
        StartCoroutine(Move());
        
    }

    private IEnumerator Move()
    {
        while (sprite.transform.localPosition != Vector3.zero)
        {
            yield return null;
            sprite.transform.localPosition = Vector3.MoveTowards(sprite.transform.localPosition, Vector3.zero, 50f);
        }

    }

    public void SetActive(bool value)
    {
        sprite.raycastTarget = value;
    }


    private void CheckResources()
    {
        int resID = (int)GameInformation.instance.allCards[cardId].nedeedResourcesType;
        int resCount = GameInformation.instance.allCards[cardId].nedeedCount;
        if (Hud.localPlayer.resources[resID] < resCount)
        {
            isActive = false;
            sprite.transform.localPosition = noActivePos;
        }
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
        (sprite.transform as RectTransform).sizeDelta = selectSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.SetSiblingIndex(slotID);
        (sprite.transform as RectTransform).sizeDelta = normalSize;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
