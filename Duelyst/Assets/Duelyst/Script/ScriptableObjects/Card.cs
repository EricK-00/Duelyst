using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardType
{
    General,
    Minion
}

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/Card", order = 1)]
public class Card : ScriptableObject
{
    [SerializeField]
    private int id;
    public int Id { get { return id; } }

    [SerializeField]
    private int cost;
    public int Cost { get { return cost; } }

    [SerializeField]
    private string cardName;
    public string CardName { get { return cardName; } }

    //public string Effect { get; private set; }

    [SerializeField]
    private CardType type;
    public CardType Type { get { return type; } }

    [SerializeField, Range(0, 99)]
    private int power;
    public int Power { get { return power; } }

    [SerializeField, Range(0, 99)]
    private int health;
    public int Health { get { return health; } }

    [SerializeField]
    private AnimatorController anim;
    public AnimatorController Anim { get { return anim; } }
}