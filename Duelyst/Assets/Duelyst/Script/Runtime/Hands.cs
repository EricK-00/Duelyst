using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    private Hand[] hands = new Hand[GameManager.MAX_HANDS];

    private void Awake()
    {
        for (int i = 0; i < hands.Length; i++)
        {
            hands[i] = transform.GetChild(i).GetComponent<Hand>();
        }
    }

    public void AddCard(Card card)
    {
        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i].NoCard)
            {
                hands[i].SetNewCard(card);
                break;
            }
        }
    }
}