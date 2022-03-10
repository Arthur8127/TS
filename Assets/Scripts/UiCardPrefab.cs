using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UiCardPrefab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image img;
    public int cardID;
    public Color[] colors;
    private int slotCurrentIndex;
    private Vector2 normalSize;

    private void Start()
    {
        MatchController.instance.onBlock += BlockCard;
    }

    private void BlockCard(bool value)
    {
        img.color = colors[1];
        img.raycastTarget = false;
        transform.localPosition = Vector2.up * -10f;
        if (value)
        {
            int resID = (int)GameInformation.instance.allCards[cardID].nedeedResourcesType;
            int count = GameInformation.instance.allCards[cardID].nedeedCount;
            if (MatchController.instance.hud.localPlayer.Resources[resID] >= count)
            {
                img.color = colors[0];
                img.raycastTarget = true;
                transform.localPosition = Vector2.zero;
            }
        }
    }

    private void OnDestroy()
    {
        MatchController.instance.onBlock -= BlockCard;
    }
    public IEnumerator MoveZirroPos()
    {
        bool isMove = true;
        while (isMove)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 60f);
            if (transform.localPosition == Vector3.zero) isMove = false;
            yield return null;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            MatchController.instance.CmdDropCard(slotCurrentIndex);
            Destroy(gameObject);
        }
        else if(eventData.button == PointerEventData.InputButton.Left)
        {
            MatchController.instance.CmdUseCard(slotCurrentIndex);
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotCurrentIndex = transform.parent.GetSiblingIndex();
        transform.parent.SetAsLastSibling();
        normalSize = (transform as RectTransform).sizeDelta;
        (transform as RectTransform).sizeDelta = normalSize * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.parent.SetSiblingIndex(slotCurrentIndex);
        (transform as RectTransform).sizeDelta = normalSize;
    }
}
