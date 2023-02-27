using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class Field : MonoBehaviour
{
    public GameObject objCanvas;
    public Card cardData;

    //
    public static List<Tile> myFieldList;
    public static List<Tile> opponentFieldList;

    private void Start()
    {
        myFieldList = new List<Tile>();
        opponentFieldList = new List<Tile>();

        Tile myStartTile, opponentStartTile;

        if (GameManager.Instance.FirstPlayer == PlayerType.ME)
        {
            Board.TryGetTile(2, 0, out myStartTile);
            Board.TryGetTile(2, Board.MaxColumn - 1, out opponentStartTile);
        }
        else
        {
            Board.TryGetTile(2, Board.MaxColumn - 1, out myStartTile);
            Board.TryGetTile(2, 0, out opponentStartTile);
        }

        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(myStartTile, cardData, true, PlayerType.ME);
        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(opponentStartTile, cardData, true, PlayerType.OPPONENT);
    }

    public static void AddTile(Tile tile, PlayerType player)
    {
        if (player == PlayerType.ME)
            myFieldList.Add(tile);
        else
            opponentFieldList.Add(tile);
    }

    public static void RemoveTile(Tile tile, PlayerType player)
    {
        if (player == PlayerType.ME)
            myFieldList.Remove(tile);
        else
            opponentFieldList.Remove(tile);
    }

    public static void ShowPlaceableTiles()
    {
        if (myFieldList.Count <= 0)
            return;

        foreach (var tile in myFieldList)
        {
            tile.ShowPlacementRange();
        }
    }

    public static void HidePlaceableTiles()
    {
        if (myFieldList.Count <= 0)
            return;

        foreach(var tile in myFieldList)
        {
            tile.HidePlacementRange();
        }
    }
}