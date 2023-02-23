using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaTile : Place
{
    private bool isManatile = true;

    public override void RegisterCard(GameObject card)
    {
        base.RegisterCard(card);

        if (isManatile)
        {
            GameManager.Instance.OnManaTileActive(PlacedObject);
            gameObject.FindChildGO(Functions.NAME_MANATILE_MANABALL).SetActive(false);
            isManatile = false;
        }
    }
}