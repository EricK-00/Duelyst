using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngineInternal;

public static partial class Functions
{
    #region GameObject tag
    public const string TAG_PLACE = "Place";
    public const string TAG_ENEMY = "Enemy";
    #endregion

    #region GameObject name
    //Managers
    public const string NAME_GAMEMANAGER = "GameManager";
    public const string NAME_UIMANAGER = "UIManager";

    //GameObject in ObjectCanvas
    public const string NAME_OBJCANVAS = "ObjectCanvas";

    public const string NAME_SELECTINGARROW = "SelectingArrow";

    public const string NAME_HAND_CARDDETAIL = "CardDetail";
    public const string NAME_HAND_CARD = "Card";

    //GameObject in UICanvas
    public const string NAME_UICANVAS = "UICanvas";

    public const string NAME_PLAYERCARDDETAIL = "PlayerCardDetail";
    public const string NAME_ENEMYCARDDETAIL = "EnemyCardDetail";

    #endregion


    #region Resource files location
    public const string FILELOC_RESOURCES_PREFAB = "Prefabs/";
    public const string FILELOC_RESOURCES_MATERIALS = "Materials/";
    #endregion

    #region Prefab name

    #endregion

    #region Prefabs

    #endregion

    #region Materials name
    public const string ASSETNAME_MATERIALS_OUTLINE = "Outline";
    #endregion

    #region Materials
    public static readonly Material outline = Resources.Load($"{FILELOC_RESOURCES_MATERIALS}{ASSETNAME_MATERIALS_OUTLINE}") as Material;
    #endregion
}