using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardType
{
    General,
    Minion
}

public class Card : ScriptableObject
{
    public int Id { get; private set; }

    public int Cost { get; private set; }
    public string Name { get; private set; }
    public string Effect { get; private set; }

    public CardType Type { get; private set; }

    public int Power { get; private set; }
    public int Health { get; private set; }

    public Animator Anim { get; private set; }
}