using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Functions.GetRootGameObject(Functions.NAME_GAMEMANAGER).GetComponent<GameManager>();
            }
                return instance;
        }
    }

    public Hands hands;
    public GameObject card;

    public const int MAX_HANDS = 6;
    public int HandsCount { get; private set; }
    public int DeckCount { get; private set; }

    private bool isPlayer1Turn = true;
    private int turnCount = 1;

    private bool inputBlock = false;

    public void EndTurn()
    {
        DrawCard();
        DrawCard();
        ChangeTurn();
    }

    public void DrawCard()
    {
        if (isPlayer1Turn)
        {
            Debug.Log("플레이어1 드로우");

            if (DeckCount <= 0)
            {
                //게임 패배
            }

            if (HandsCount < MAX_HANDS)
            {
                hands.AddCard(card, 3);
                ++HandsCount;
                --DeckCount;
            }
        }
        else
        {
            Debug.Log("플레이어2 드로우");
        }
    }

    public void ReduceHandsCount()
    {
        --HandsCount;
    }

    private void ChangeTurn()
    {
        ++turnCount;
        //isPlayer1Turn = !isPlayer1Turn;
        //턴 변경 UI 띄우기
    }
}