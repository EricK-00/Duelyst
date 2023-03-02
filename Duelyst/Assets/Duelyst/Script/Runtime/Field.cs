using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
//using Mono.Collections.Generic;

public class Field : MonoBehaviour
{
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