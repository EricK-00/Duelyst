using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int MAX_ROW = 5;
    public static int MaxColumn { get; set; }

    private GameObject rowPrefab;

    private static Tile[,] board;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        rowPrefab = Functions.ROW;
        MaxColumn = rowPrefab.transform.childCount;
        board = new Tile[MAX_ROW, MaxColumn];

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = transform.GetChild(i).GetChild(j).GetComponent<Tile>();
            }
        }

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
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
}
