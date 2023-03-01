using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaTile : Tile
{
    protected override void Awake()
    {
        base.Awake();
        HaveMana = true;
    }

    public override void OnPlaceEffect()
    {
        base.OnPlaceEffect();

        if (HaveMana)
        {
            GameManager.Instance.OnManaTileActive(PlacedObject);
            gameObject.FindChildGO(Functions.NAME__MANA_TILE__MANA_BALL).SetActive(false);
            HaveMana = false;
        }
    }
}