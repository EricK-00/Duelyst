using UnityEngine;
using UnityEngine.Rendering;

#region EnumTypes
namespace EnumTypes
{
    public enum PlayerType
    {
        ME = 0,
        OPPONENT
    }

    public enum PlacedObjType
    {
        BLANK = 0,
        ALLY,
        ENEMY
    }

    public enum ActiveType
    {
        PLACEABLE = 0,
        MOVABLE,
        ATTACKABLE
    }

    public enum PlayingCardDirection
    {
        Left = 180,
        Right = 0
    }
}
#endregion

public static partial class Functions
{
    #region UI text
    public const string TEXT__OPPONENT = "OPPONENT";
    #endregion

    #region AnimationState name
    public const string NAME__ANIMATION_STATE__DRAW = "Card_Draw";
    public const string NAME__ANIMATION_STATE__TURN_START = "TurnStartUI_Start";
    public const string NAME__ANIMATION_STATE__GAME_OVER = "GameOverUI_Shake";

    #endregion

    #region GameObject tag
    public const string TAG__UNTAGGED = "Untagged";
    public const string TAG__TILE = "Tile";
    #endregion

    #region GameObject name
    //Managers
    public const string NAME__GAME_MANAGER = "GameManager";
    public const string NAME__UI_MANAGER = "UIManager";
    public const string NAME__PLAYING_CARD_POOL = "PlayingCardPool";

    //AI
    public const string NAME__AI = "AI";

    //GameObject in BgCanvas
    public const string NAME__MANA_TILE__MANA_BALL = "ManaBall";

    //GameObject in ObjectCanvas
    public const string NAME__OBJ_CANVAS = "ObjectCanvas";

    public const string NAME__LAYER = "Layer";//Layer + number
    public const string NAME__MY_GENERAL = "MyGeneral";
    public const string NAME__OPPONENT_GENERAL = "OpponentGeneral";

    public const string NAME__PLAYING_CARD__CARD_SPRITE = "CardSprite";
    public const string NAME__PLAYING_CARD__POWER_TEXT = "PowerText";
    public const string NAME__PLAYING_CARD__HEALTH_TEXT = "HealthText";

    //GameObject in UICanvas
    public const string NAME__UI_CANVAS = "UICanvas";

    public const string NAME__LPLAYER_UI = "LeftPlayerUI";
    public const string NAME__RPLAYER_UI = "RightPlayerUI";
    public const string NAME__PLAYER_UI__NAME_TEXT = "NameText";
    public const string NAME__PLAYER_UI__HP_TEXT = "HealthText";

    public const string NAME__PLAYER_UI_MY_MANA_UI = "MyMana";
    public const string NAME__PLAYER_UI__OPPONENT_MANA_UI = "OpponentMana";
    public const string NAME__PLAYER_UI__OPPONENT_HANDS_UI = "OpponentHands";
    public const string NAME__PLAYER_UI__OPPONENT_DECK_UI = "OpponentDeck";

    public const string NAME__PLAYER_UI__MANA_IMAGES = "ManaImages";
    public const string NAME__PLAYER_UI__MANA_TEXT = "CurrentManaText";
    public const string NAME__PLAYER_UI__MAX_MANA_TEXT = "MaxManaText";
    public const string NAME__PLAYER_UI__HANDS_TEXT = "CurrentHandsText";
    public const string NAME__PLAYER_UI__DECK_TEXT = "CurrentDeckText";

    public const string NAME__MY_DECK_UI = "MyDeckUI";
    public const string NAME__MY_DECK_TEXT = "CurrentDeckText";

    public const string NAME__HANDS = "ActionBar";
    public const string NAME__HAND__CARD_SPRITE = "CardSprite";
    public const string NAME__HAND__COST_TEXT = "CostText";
    public const string NAME__HAND__DRAW_ANIM = "DrawAnim";

    public const string NAME__CARD_DETAIL = "CardDetail";
    public const string NAME__CARD_DETAIL__SPRITE = "Sprite";
    public const string NAME__CARD_DETAIL__POWER = "Power";
    public const string NAME__CARD_DETAIL__HEALTH = "Health";
    public const string NAME__CARD_DETAIL__TYPE = "Type";
    public const string NAME__CARD_DETAIL__NAME = "Name";
    public const string NAME__CARD_DETAIL__COST = "Cost";

    public const string NAME__PLACE_ANIM = "PlaceAnim";
    public const string NAME__YOUR_TURN_UI = "YourTurnUI";
    public const string NAME__ENEMY_TURN_UI = "EnemyTurnUI";
    public const string NAME__SELECTING_ARROW = "SelectingArrow";
    public const string NAME__GAME_OVER_UI_VICTORY = "GameOverUI_Victory";
    public const string NAME__GAME_OVER_UI_DEFEAT = "GameOverUI_Defeat";
    public const string NAME__GAME_OVER_UI_DRAW = "GameOverUI_Draw";

    #endregion

    #region Resource file location
    public const string FILELOC__RESOURCES__PREFAB = "Prefabs/";
    public const string FILELOC__RESOURCES__MATERIALS = "Materials/";
    public const string FILELOC__RESOURCES__SO__CARD = "ScriptableObjects/Card/";
    #endregion

    #region Resource name
    //Prefabs
    public const string ASSETNAME__PREFABS__ROW = "Row";
    public const string ASSETNAME__PREFABS__TILE = "Tile";
    public const string ASSETNAME__PREFABS__PLAYING_CARD = "PlayingCard";

    //Materials
    public const string ASSETNAME__MATERIALS__ALLYOUTLINE = "AllyOutline";
    public const string ASSETNAME__MATERIALS__ENEMYOUTLINE = "EnemyOutline";

    //Scriptable Objects
    //Card
    public const string ASSETNAME_SO__CARD__VAATH_THE_IMMORTAL = "VaathTheImmortal";
    public const string ASSETNAME_SO__CARD__PHALANXAR = "Phalanxar";
    #endregion

    #region Resources
    //Prefab resources
    public static readonly GameObject ROW = Resources.Load($"{FILELOC__RESOURCES__PREFAB}{ASSETNAME__PREFABS__ROW}") as GameObject;
    public static readonly GameObject TILE = Resources.Load($"{FILELOC__RESOURCES__PREFAB}{ASSETNAME__PREFABS__TILE}") as GameObject;
    public static readonly GameObject PLAYING_CARD = Resources.Load($"{FILELOC__RESOURCES__PREFAB}{ASSETNAME__PREFABS__PLAYING_CARD}") as GameObject;

    //Material resources
    public static readonly Material ALLY_OUTLINE = Resources.Load($"{FILELOC__RESOURCES__MATERIALS}{ASSETNAME__MATERIALS__ALLYOUTLINE}") as Material;
    public static readonly Material ENEMY_OUTLINE = Resources.Load($"{FILELOC__RESOURCES__MATERIALS}{ASSETNAME__MATERIALS__ENEMYOUTLINE}") as Material;

    //Scriptable Object Resources
    //Card
    public static readonly Card VAATH_THE_IMMORTAL = Resources.Load($"{FILELOC__RESOURCES__SO__CARD}{ASSETNAME_SO__CARD__VAATH_THE_IMMORTAL}") as Card;
    public static readonly Card PHALANXAR = Resources.Load($"{FILELOC__RESOURCES__SO__CARD}{ASSETNAME_SO__CARD__PHALANXAR}") as Card;
    #endregion
}