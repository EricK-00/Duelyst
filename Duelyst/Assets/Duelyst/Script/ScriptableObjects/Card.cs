using UnityEngine;
using EnumTypes;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/Card", order = 1)]
public class Card : ScriptableObject
{
    [SerializeField]
    private int _id;
    public int Id { get { return _id; } }

    [SerializeField]
    private int _cost;
    public int Cost { get { return _cost; } }

    [SerializeField]
    private string _name;
    public string Name { get { return _name; } }

    [SerializeField]
    private CardType _type;
    public CardType Type { get { return _type; } }

    [SerializeField, Range(0, 99)]
    private int _power;
    public int Power { get { return _power; } }

    [SerializeField, Range(0, 99)]
    private int _health;
    public int Health { get { return _health; } }

    [SerializeField]
    private AnimatorOverrideController _anim;
    public AnimatorOverrideController Anim { get { return _anim; } }

    [SerializeField]
    private string _description;
    public string Description { get { return _description; } }

    [SerializeField]
    private bool _isRush;
    public bool IsRush { get { return _isRush; } }

}