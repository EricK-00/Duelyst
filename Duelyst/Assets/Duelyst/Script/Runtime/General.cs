using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General : PlayingCard
{
    public override void SetUp(Card card, PlayerType owner, int row, bool isRush)
    {
        base.SetUp(card, owner, row, isRush);
    }

    private void OnEnable()
    {
        IsGeneral = true;
    }

    private void OnDisable()
    {
        //GameOver()
    }
}
