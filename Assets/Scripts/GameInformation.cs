using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInformation : MonoBehaviour
{
    public static GameInformation instance;
    public List<CardBase> allCards = new List<CardBase>();

    private void Awake()
    {
        instance = this;
    }
    public List<CardBase> GetCards()
    {
        List<CardBase> cards = new List<CardBase>();
        for (int i = 0; i < allCards.Count; i++)
            cards.Add(allCards[i].GetClone());

        for (int i = 0; i < cards.Count; i++)
        {
            CardBase currentCard = cards[i];
            int randIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randIndex];
            cards[randIndex] = currentCard;

        }
        return cards;
    }
}
[System.Serializable]
public class CardBase
{
    public int index;
    public Sprite sprite;
    public enum NedeedType { Recruts = 0, Gems = 1, Bricks = 2 }
    public NedeedType nedeedResourcesType;
    public int nedeedCount;
    public bool isDropped = true;
    public List<ActionBase> AllActions = new List<ActionBase>();

    public CardBase GetClone()
    {
        return (CardBase)MemberwiseClone();
    }
}
public class MessageParam
{

}

