using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class General : PlayingCard
{
    public UnityEvent<PlayerType, int> healthUpdateEvent = new UnityEvent<PlayerType, int>();

    protected override void SetHealth(int currentHealth)
    {
        base.SetHealth(currentHealth);
        if (healthUpdateEvent != null)
            healthUpdateEvent.Invoke(Owner, Health);
    }
}