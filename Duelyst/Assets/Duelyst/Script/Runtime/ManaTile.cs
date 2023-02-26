using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaTile : Tile
{
    private bool isManatile = true;

    public override void OnPlaceEffect()
    {
        if (isManatile)
        {
            GameManager.Instance.OnManaTileActive(PlacedObject);
            gameObject.FindChildGO(Functions.NAME_MANATILE_MANABALL).SetActive(false);
            isManatile = false;
        }
    }
}