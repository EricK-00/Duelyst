using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    private Hand[] myHands = new Hand[GameManager.MAX_HANDS];

    private List<Card> opponentHands = new List<Card>();

    private void Awake()
    {
        for (int i = 0; i < myHands.Length; i++)
        {
            myHands[i] = transform.GetChild(i).GetComponent<Hand>();
        }
    }

    public void AddCard(Card card)
    {
        for (int i = 0; i < myHands.Length; i++)
        {
            if (myHands[i].NoCard)
            {
                myHands[i].SetNewCard(card);
                break;
            }
        }
    }
}