using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class Field : MonoBehaviour
{
    public GameObject objCanvas;
    public Card cardData;

    private static List<Tile> myFieldList;
    private static List<Tile> opponentFieldList;

    private void Start()
    {
        myFieldList = new List<Tile>();
        opponentFieldList = new List<Tile>();

        Tile startTile;

        if (GameManager.Instance.FirstPlayer == PlayerType.ME)
        {
            Board.TryGetTile(2, 0, out startTile);
        }
        else
        {
            Board.TryGetTile(2, Board.MaxColumn - 1, out startTile);
        }

        //GameObject playingCardInst = Instantiate(Functions.PLAYINGCARD, objCanvas.transform);
        //playingCardInst.transform.position = startTile.transform.position;

        //PlayingCard playingCard = playingCardInst.GetComponent<PlayingCard>();
        //playingCard.SetUp(cardData, startTile.GetRow(), true);

        //startTile.RegisterCard(playingCardInst);

        PlayingCardPoolingManager.Instance.Active(startTile, cardData, true);
    }

    public static void RegisterTile(Tile tile)
    {
        myFieldList.Add(tile);
    }

    public static void UnregisterTile(Tile tile)
    {
        myFieldList.Remove(tile);
    }

    public static void ShowPlaceableTiles()
    {
        foreach (var tile in myFieldList)
        {
            tile.ShowPlacementRange();
        }
    }

    public static void HidePlaceableTiles()
    {
        foreach(var tile in myFieldList)
        {
            tile.HidePlacementRange();
        }
    }
}