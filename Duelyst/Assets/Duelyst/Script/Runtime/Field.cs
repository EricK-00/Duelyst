using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
//using Mono.Collections.Generic;

public class Field : MonoBehaviour
{
    //
    public Card cardData;
    public Card cardData_general;

    public static bool IsPlaceableShowing { get; private set; }

    private static List<Tile> _myFieldList;
    public static ReadOnlyCollection<Tile> MyFieldList { get { return _myFieldList.AsReadOnly(); } }
    private static List<Tile> _opponentFieldList;
    public static ReadOnlyCollection<Tile> OpponentFieldList { get { return _opponentFieldList.AsReadOnly(); } }

    private void Awake()
    {
        _myFieldList = new List<Tile>();
        _opponentFieldList = new List<Tile>();
    }

    private void Start()
    {
        //
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

        PlayingCardPoolingManager.Instance.ActiveNewCard(myStartTile, cardData_general, PlayerType.ME);
        PlayingCardPoolingManager.Instance.ActiveNewCard(opponentStartTile, cardData_general, PlayerType.OPPONENT);

        //
        PlayingCardPoolingManager.Instance.ActiveNewCard(aiTestOppoentTile1, cardData, PlayerType.ME);
        PlayingCardPoolingManager.Instance.ActiveNewCard(aiTestOppoentTile2, cardData, PlayerType.ME);
        PlayingCardPoolingManager.Instance.ActiveNewCard(aiTestOppoentTile3, cardData, PlayerType.ME);
    }

    public static void AddPlayerTile(Tile tile, PlayerType player)
    {
        if (player == PlayerType.ME)
            _myFieldList.Add(tile);
        else
            _opponentFieldList.Add(tile);
    }

    public static void RemovePlayerTile(Tile tile, PlayerType player)
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

        IsPlaceableShowing = true;
        foreach (var tile in _myFieldList)
        {
            tile.ShowPlacementRange();
        }
    }

    public static void HidePlaceableTiles()
    {
        IsPlaceableShowing = false;
        if (_myFieldList.Count <= 0)
            return;

        foreach (var tile in _myFieldList)
        {
            tile.HidePlacementRange();
        }
    }
}