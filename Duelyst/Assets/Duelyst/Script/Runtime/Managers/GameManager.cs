using System.Collections;
using System.Collections.Generic;
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

    private bool isPlayer1Turn = true;
    private int turnCount = 1;

    public void EndTurn()
    {
        DrawCard();
        ChangeTurn();
    }

    private void DrawCard()
    {
        if (isPlayer1Turn)
        {
            Debug.Log("플레이어1 드로우");
        }
        else
        {
            Debug.Log("플레이어2 드로우");
        }
    }

    private void ChangeTurn()
    {
        ++turnCount;
        isPlayer1Turn = !isPlayer1Turn;
        //턴 변경 UI 띄우기
    }
}