using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private static Card[] deckList;
    private static int nextCardIndex;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        deckList = new Card[GameManager.MAX_DECK];
        nextCardIndex = GameManager.MAX_DECK - 1;

        SetData();
        Suffle();
    }

    private void SetData()
    {

    }

    private void Suffle()
    {
        for (int i =0; i < deckList.Length; i++)
        {
            
        }
    }

    public bool TryGetDeckTop(out Card card)
    {
        card = null;
        if (nextCardIndex < 0)
            return false;

        card = deckList[nextCardIndex--];
        return true;
    }

    public bool TryGetRandomCard(out Card card)
    {
        card = null;
        if (nextCardIndex < 0)
            return false;

        card = deckList[Random.Range(0, nextCardIndex + 1)];
        return true;
    }
}