using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
//using Mono.Collections.Generic;

public class Field : MonoBehaviour
{
    public GameObject objCanvas;
    public Card cardData;

    //
    private static List<Tile> _myFieldList;
    public static ReadOnlyCollection<Tile> MyFieldList { get { return _myFieldList.AsReadOnly(); } }
    private static List<Tile> _opponentFieldList;
    public static ReadOnlyCollection<Tile> OpponentFieldList { get { return _opponentFieldList.AsReadOnly(); } }

    private void Start()
    {
        _myFieldList = new List<Tile>();
        _opponentFieldList = new List<Tile>();

        Tile myStartTile, opponentStartTile;

        //
        Tile aiTestOppoentTile1;
        Tile aiTestOppoentTile2;
        Tile aiTestOppoentTile3;

        if (GameManager.Instance.FirstPlayer == PlayerType.ME)
        {
            Board.TryGetTile(2, 0, out myStartTile);
            Board.TryGetTile(2, Board.MaxColumn - 1, out opponentStartTile);

            //
            Board.TryGetTile(3, Board.MaxColumn - 1, out aiTestOppoentTile1);
            Board.TryGetTile(1, Board.MaxColumn - 1, out aiTestOppoentTile2);
            Board.TryGetTile(4, Board.MaxColumn - 3, out aiTestOppoentTile3);
        }
        else
        {
            Board.TryGetTile(2, Board.MaxColumn - 1, out myStartTile);
            Board.TryGetTile(2, 0, out opponentStartTile);

            //
            Board.TryGetTile(3, 1, out aiTestOppoentTile1);
            Board.TryGetTile(1, 1, out aiTestOppoentTile2);
            Board.TryGetTile(4, 2, out aiTestOppoentTile3);
        }

        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(myStartTile, cardData, true, PlayerType.ME);
        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(opponentStartTile, cardData, true, PlayerType.OPPONENT);

        //
        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(aiTestOppoentTile1, cardData, true, PlayerType.OPPONENT);
        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(aiTestOppoentTile2, cardData, true, PlayerType.OPPONENT);
        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(aiTestOppoentTile3, cardData, true, PlayerType.OPPONENT);
    }

    public static void AddTile(Tile tile, PlayerType player)
    {
        if (player == PlayerType.ME)
            _myFieldList.Add(tile);
        else
            _opponentFieldList.Add(tile);
    }

    public static void RemoveTile(Tile tile, PlayerType player)
    {
        if (player == PlayerType.ME)
            _myFieldList.Remove(tile);
        else
            _opponentFieldList.Remove(tile);
    }

    public static void ShowPlaceableTiles()
    {
        if (_myFieldList.Count <= 0)
            return;

        foreach (var tile in _myFieldList)
        {
            tile.ShowPlacementRange();
        }
    }

    public static void HidePlaceableTiles()
    {
        if (_myFieldList.Count <= 0)
            return;

        foreach(var tile in _myFieldList)
        {
            tile.HidePlacementRange();
        }
    }
}