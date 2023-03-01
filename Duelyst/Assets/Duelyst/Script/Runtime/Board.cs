using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int MAX_ROW = 5;
    public static int MaxColumn { get; private set; }

    private GameObject rowPrefab;

    private static Tile[,] board;
    private static List<Tile> manaTiles;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        rowPrefab = Functions.ROW;
        MaxColumn = rowPrefab.transform.childCount;

        board = new Tile[MAX_ROW, MaxColumn];
        manaTiles = new List<Tile>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                //타일 저장
                board[i, j] = transform.GetChild(i).GetChild(j).GetComponent<Tile>();

                //마나타일 저장
                if (board[i, j].GetComponent<ManaTile>() != null)
                    manaTiles.Add(board[i, j]);
            }
        }

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                //자신과 주변 타일의 인덱스 저장
                board[i, j].InitializeIndex(i, j);
            }
        }
    }

    public static bool TryGetTile(int row, int col, out Tile tile)
    {
        tile = null;

        if (row >= 0 && row < MAX_ROW && col >= 0 && col < MaxColumn)
        {
            tile = board[row, col];
            return true;
        }

        return false;
    }

    public static (int, int)[] GetManaTilePos()
    {
        //마나타일 업데이트
        for (int i = 0; i < manaTiles.Count; i++)
        {
            if (!manaTiles[i].HaveMana)
            {
                manaTiles.Remove(manaTiles[i]);
                --i;
            }
        }

        //마나타일 위치 반환
        if (manaTiles.Count <= 0)
        {
            return null;
        }
        else
        {
            (int, int)[] manaTilePos = new (int, int)[manaTiles.Count];
            for (int i = 0; i < manaTilePos.Length; i++)
            {
                manaTilePos[i] = (manaTiles[i].Row, manaTiles[i].Column);
            }

            return manaTilePos;
        }
    }
}
