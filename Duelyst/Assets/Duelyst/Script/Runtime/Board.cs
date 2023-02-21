using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int MAX_ROW = 5;
    public static int MaxColumn { get; set; }

    private GameObject rowPrefab;

    private static Place[,] board;

    private void Awake()
    {
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        rowPrefab = Functions.ROW;
        MaxColumn = rowPrefab.transform.childCount;
        board = new Place[MAX_ROW, MaxColumn];

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = transform.GetChild(i).GetChild(j).GetComponent<Place>();
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

    public static bool TryGetPlace(int row, int col, out Place place)
    {
        place = null;

        if (row >= 0 && row < MAX_ROW && col >= 0 && col < MaxColumn)
        {
            place = board[row, col];
            return true;
        }
        
        return false;
    }
}
