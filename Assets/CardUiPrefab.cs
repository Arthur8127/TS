using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class CardUiPrefab : NetworkBehaviour
{
    public Image img;
    public void Setup(int id)
    {
        img.sprite = GameInformation.instance.allCards[id].sprite;
    }

    public IEnumerator MoveInParent()
    {
        while(transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, 30f);
            yield return null;
        }
    }

   
}
