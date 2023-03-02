using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EnumTypes;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private readonly Color COLOR_WHITE = new Color(1, 1, 1, 0.3f); //white
    private readonly Color COLOR_YELLOW = new Color(1, 1, 0, 0.3f);//yellow
    private readonly Color COLOR_GREEN = new Color(0.1f, 1, 0, 0.3f);//green

    public int Row { get; private set; } = -1;
    public int Column { get; private set; } = -1;
    [SerializeField]
    private List<Tile> aroundTiles = new List<Tile>();
    [SerializeField]
    private List<Tile> oneDistanceTiles = new List<Tile>();

    private RectTransform tileRect;

    private Image tileImage;
    private Color tileDefaultColor;

    public PlayingCard Card { get; private set; }

    public PlacedObjType PlacedObject { get; private set; } = PlacedObjType.BLANK;

    public bool IsPlaceable { get; private set; } = false;
    public bool isMovable { get; private set; } = false;
    public bool isAttackable { get; private set; } = false;

    public bool HaveMana { get; protected set; } = false;

    protected virtual void Awake()
    {
        tileRect = GetComponent<RectTransform>();

        tileImage = GetComponent<Image>();
        tileDefaultColor = tileImage.color;
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        tileImage.fillCenter = false;
        if (PlacedObject == PlacedObjType.BLANK)
            return;

        //카드 상세보기
        UIManager.Instance.ShowPlayingCardDetail(Card);

        Card.ShowOutline();
    }

    public void OnPointerExit(PointerEventData ped)
    {
        tileImage.fillCenter = true;
        if (PlacedObject == PlacedObjType.BLANK)
            return;

        //카드 상세보기 종료
        UIManager.Instance.HidePlayingCardDetails();

        Card.HideOutline();
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        if (PlacedObject != PlacedObjType.ALLY || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
            return;

        //드래그 시작
        UIManager.Instance.ShowSelectingArrow(tileRect);

        if (Card != null && Card.MoveChance > 0)
            ShowMoveRange();

        if (Card != null && Card.AttackChance > 0)
            ShowAttackRange();
    }

    public void OnDrag(PointerEventData ped)
    {
        /* Do nothing */
    }

    public void OnEndDrag(PointerEventData ped)
    {
        //드래그 종료
        UIManager.Instance.HideSelectingArrow();

        if (PlacedObject != PlacedObjType.ALLY || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
        {
            HideAttackRange();
            HideMoveRange();

            return;
        }

        //현재 레이캐스트 결과 가져오기
        Tile targetTile = ped.pointerCurrentRaycast.gameObject?.GetComponent<Tile>();
        if (targetTile == null || GameManager.Instance.TaskBlock)
        {
            HideAttackRange();
            HideMoveRange();

            return;
        }

        if (targetTile.isAttackable)
        {
            BattleWithTarget(targetTile);
        }
        else if (targetTile.isMovable)
        {
            MoveCard(targetTile);
        }

        HideAttackRange();
        HideMoveRange();
    }

    private void BattleWithTarget(Tile attackTarget)
    {
        StartCoroutine(GameManager.Instance.PlayTask(Card.Battle(attackTarget.Card, Column, attackTarget.Column)));
    }

    private void MoveCard(Tile moveTarget)
    {
        PlayingCard targetCard = Card.GetComponent<PlayingCard>();

        //카드 등록 장소 변경
        ChangeTile(targetCard, moveTarget);

        //이동
        StartCoroutine(GameManager.Instance.PlayTask(targetCard.Move(this, moveTarget)));
    }

    public void ChangeTile(PlayingCard targetCard, Tile moveTarget)
    {
        if (targetCard.Data == null)
            return;

        UnregisterCard();
        moveTarget.RegisterCard(targetCard);
    }

    public virtual void OnPlaceEffect()
    {
        PlacedObjType foeObj = Card.Owner == PlayerType.ME ? PlacedObjType.ENEMY : PlacedObjType.ALLY;
        CheckExhausted(foeObj);
    }

    public void RegisterCard(PlayingCard card)
    {
        if (PlacedObject != PlacedObjType.BLANK)
            return;

        Card = card;
        Card.SetLayer(Row);
        Card.deathEvent.AddListener(UnregisterCard);

        PlacedObject = Card.Owner == PlayerType.ME ? PlacedObjType.ALLY : PlacedObjType.ENEMY;

        Field.AddPlayerTile(this, Card.Owner);

        if (GameManager.Instance.CurrentTurnPlayer != Card.Owner)
        {
            foreach (var tile in aroundTiles)
            {
                if (tile.Card != null && tile.Card.Owner != Card.Owner)
                    tile.CheckRefreshed(PlacedObject);
            }
        }
    }

    public void UnregisterCard()
    {
        PlayerType beforeCardOwner = Card.Owner;
        PlacedObjType beforePlacedObject = PlacedObject;

        Card.deathEvent.RemoveListener(UnregisterCard);
        Card = null;
        PlacedObject = PlacedObjType.BLANK;

        bool isHandsDragging = Field.IsPlaceableShowing;
        if (isHandsDragging)
            Field.HidePlaceableTiles();

        Field.RemovePlayerTile(this, beforeCardOwner);

        if (isHandsDragging)
            Field.ShowPlaceableTiles();

        if (GameManager.Instance.CurrentTurnPlayer != beforeCardOwner)
        {
            foreach (var tile in aroundTiles)
            {
                if (tile.Card != null && tile.Card.Owner != beforeCardOwner)
                    tile.CheckExhausted(beforePlacedObject);
            }
        }

    }

    public void InitializeIndex(int tileRow, int tileCol)
    {
        if (Row != -1 && Column != -1)
            return;

        Row = tileRow;
        Column = tileCol;

        int row, col;
        for (int i = -1; i <= 1; i++)
        {
            row = Row + i;
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                col = Column + j;

                Tile tile;
                if (Board.TryGetTile(row, col, out tile))
                {
                    aroundTiles.Add(tile);

                    if (Mathf.Abs(i) + Mathf.Abs(j) == 1)
                    {
                        oneDistanceTiles.Add(tile);
                    }
                }
            }
        }
    }

    public void ShowPlacementRange()
    {
        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject == PlacedObjType.BLANK)
                tile.ActiveTile(ActiveType.PLACEABLE);
        }
    }

    public void HidePlacementRange()
    {
        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject != PlacedObjType.ENEMY)
                tile.InactiveTile();
        }
    }

    public void ShowAttackRange()
    {
        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject == PlacedObjType.ENEMY)
                tile.ActiveTile(ActiveType.ATTACKABLE);
        }
    }

    public void HideAttackRange()
    {
        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject == PlacedObjType.ENEMY)
                tile.InactiveTile();
        }
    }

    public void ShowMoveRange()
    {
        ShowOneDistanceRange();
        foreach (var tile in oneDistanceTiles)
        {
            if (tile.PlacedObject != PlacedObjType.ENEMY)
                tile.ShowOneDistanceRange();
        }
    }

    public void HideMoveRange()
    {
        HideOneDistanceRange();
        foreach (var tile in oneDistanceTiles)
        {
            if (tile.PlacedObject != PlacedObjType.ENEMY)
                tile.HideOneDistanceRange();
        }
    }

    public void ShowOneDistanceRange()
    {
        foreach (var tile in oneDistanceTiles)
        {
            if (tile.PlacedObject == PlacedObjType.BLANK)
                tile.ActiveTile(ActiveType.MOVABLE);
        }
    }

    public void HideOneDistanceRange()
    {
        foreach (var tile in oneDistanceTiles)
        {
            tile.InactiveTile();
        }
    }

    private void ActiveTile(ActiveType activeType)
    {
        switch (activeType)
        {
            case ActiveType.PLACEABLE:
                tileImage.color = COLOR_GREEN;
                IsPlaceable = true;
                break;

            case ActiveType.MOVABLE:
                tileImage.color = COLOR_WHITE;
                isMovable = true;
                break;

            case ActiveType.ATTACKABLE:
                tileImage.color = COLOR_YELLOW;
                isAttackable = true;
                break;
        }
    }

    private void InactiveTile()
    {
        tileImage.color = tileDefaultColor;

        isAttackable = false;
        isMovable = false;
        IsPlaceable = false;
    }
    
    private void CheckExhausted(PlacedObjType foeObject)
    {
        if (Card == null)
            return;

        if (Card.MoveChance > 0)
            return;

        if (Card.AttackChance <= 0)
        {
            Card.PaintGray();
            return;
        }

        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject == foeObject)
            {
                return;
            }
        }

        Card.PaintGray();
    }

    private void CheckRefreshed(PlacedObjType foeObject)
    {
        if (Card == null)
            return;

        if (Card.AttackChance <= 0)
            return;

        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject == foeObject)
            {
                Card.PaintDefault();
                return;
            }
        }
    }
}