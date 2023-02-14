using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardType
{
    Minion,
    General
}

public class Card : MonoBehaviour
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Effect { get; private set; }

    public CardType Type { get; private set; }

    public int Attack { get; private set; }
    public int Health { get; private set; }
}