using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngineInternal;

public static partial class Functions
{
    #region UI text
    public const string TEXT_OPPONENT = "OPPONENT";
    #endregion

    #region AnimationState name
    #endregion

    #region GameObject tag
    public const string TAG_UNTAGGED = "Untagged";
    public const string TAG_PLACE = "Place";
    #endregion

    #region GameObject name
    //Managers
    public const string NAME_GAMEMANAGER = "GameManager";
    public const string NAME_UIMANAGER = "UIManager";

    //GameObject in BgCanvas
    public const string NAME_MANATILE_MANABALL = "ManaBall";

    //GameObject in ObjectCanvas
    public const string NAME_OBJCANVAS = "ObjectCanvas";

    public const string NAME_LAYER = "Layer";//Layer + number
    public const string NAME_PLAYINGCARD_CARDSPRITE = "CardSprite";

    //GameObject in UICanvas
    public const string NAME_UICANVAS = "UICanvas";

    public const string NAME_LPLAYERUI = "LeftPlayerUI";
    public const string NAME_RPLAYERUI = "RightPlayerUI";
    public const string NAME_PLAYERUI_NAME = "NameText";
    public const string NAME_PLAYERUI_HP = "HealthText";

    public const string NAME_PLAYERUI_MYMANAUI = "MyMana";
    public const string NAME_PLAYERUI_OPPONENTMANAUI = "OpponentMana";
    public const string NAME_PLAYERUI_OPPONENTHANDSUI = "OpponentHands";
    public const string NAME_PLAYERUI_OPPONENTDECKUI = "OpponentDeck";

    public const string NAME_PLAYERUI_MANAIMAGES = "ManaImages";
    public const string NAME_PLAYERUI_MANATEXT = "CurrentManaText";
    public const string NAME_PLAYERUI_HANDSTEXT = "CurrentHandsText";
    public const string NAME_PLAYERUI_DECKTEXT = "CurrentDeckText";

    public const string NAME_MYDECKUI = "MyDeckUI";
    public const string NAME_MYDECKTEXT = "CurrentDeckText";

    public const string NAME_SELECTINGARROW = "SelectingArrow";

    public const string NAME_HAND_CARDDETAIL = "CardDetail";
    public const string NAME_HAND_CARDSPRITE = "CardSprite";
    public const string NAME_HAND_COSTTEXT = "CostText";
    public const string NAME_HAND_DRAWANIM = "DrawAnim";

    public const string NAME_HANDS = "ActionBar";

    public const string NAME_PLAYERCARDDETAIL = "PlayerCardDetail";
    public const string NAME_ENEMYCARDDETAIL = "EnemyCardDetail";

    public const string NAME_YOURTURN = "YourTurnUI";
    public const string NAME_ENEMYTURN = "EnemyTurnUI";

    #endregion


    #region Resource files location
    public const string FILELOC_RESOURCES_PREFAB = "Prefabs/";
    public const string FILELOC_RESOURCES_MATERIALS = "Materials/";
    #endregion

    #region Prefab name
    public const string ASSETNAME_PREFABS_ROW = "Row";
    public const string ASSETNAME_PREFABS_PLACE = "Place";
    public const string ASSETNAME_PREFABS_PLAYINGCARD = "PlayingCard";
    #endregion

    #region Prefabs
    public static readonly GameObject ROW = Resources.Load($"{FILELOC_RESOURCES_PREFAB}{ASSETNAME_PREFABS_ROW}") as GameObject;
    public static readonly GameObject PLACE = Resources.Load($"{FILELOC_RESOURCES_PREFAB}{ASSETNAME_PREFABS_PLACE}") as GameObject;
    public static readonly GameObject PLAYINGCARD = Resources.Load($"{FILELOC_RESOURCES_PREFAB}{ASSETNAME_PREFABS_PLAYINGCARD}") as GameObject;

    #endregion

    #region Materials name
    public const string ASSETNAME_MATERIALS_OUTLINE = "Outline";
    #endregion

    #region Materials
    public static readonly Material OUTLINE = Resources.Load($"{FILELOC_RESOURCES_MATERIALS}{ASSETNAME_MATERIALS_OUTLINE}") as Material;
    #endregion
}