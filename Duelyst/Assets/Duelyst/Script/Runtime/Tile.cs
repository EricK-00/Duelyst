using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EnumTypes;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private readonly Color COLOR_WHITE = new Color(1, 1, 1, 0.1f); //white
    private readonly Color COLOR_YELLOW = new Color(1, 1, 0, 0.3f);//yellow

    public int RowIndex { get; private set; } = -1;
    public int ColumnIndex { get; private set; } = -1;
    [SerializeField]
    private List<Tile> aroundTiles = new List<Tile>();
    [SerializeField]
    private List<Tile> oneDistanceTiles = new List<Tile>();

    private RectTransform tileRect;

    private Image tileImage;
    private Color tileDefaultColor;

    public PlayingCard Card { get; private set; }
    private Image cardImage;
    private Animator cardAnimator;

    private Material cardDefaultMat;
    private Material allyOutline;
    private Material enemyOutline;

    public PlacedObjType PlacedObject = PlacedObjType.BLANK; //{ get; private set; } = PlacedObj.BLANK;

    public bool IsPlaceable { get; private set; } = false;
    public bool isMovable { get; private set; } = false;
    public bool isAttackable { get; private set; } = false;

    public bool HaveMana { get; protected set; } = false;

    public int debug_Weight = 0;

    private void Awake()
    {
        tileRect = GetComponent<RectTransform>();

        tileImage = GetComponent<Image>();
        tileDefaultColor = tileImage.color;

        allyOutline = Functions.ALLY_OUTLINE;
        enemyOutline = Functions.ENEMY_OUTLINE;
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        tileImage.fillCenter = false;
        if (PlacedObject == PlacedObjType.BLANK)
            return;

        //카드 상세보기
        UIManager.Instance.ShowPlayerCardDetail(cardAnimator, 
           PlacedObject == PlacedObjType.ALLY ? PlayerType.ME : PlayerType.OPPONENT);

        cardImage.material = PlacedObject == PlacedObjType.ALLY ? allyOutline : enemyOutline;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        tileImage.fillCenter = true;
        if (PlacedObject == PlacedObjType.BLANK)
            return;

        //카드 상세보기 종료
        UIManager.Instance.DisableCardDetails();

        cardImage.material = cardDefaultMat;
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

    }

    public void OnEndDrag(PointerEventData ped)
    {
        if (PlacedObject != PlacedObjType.ALLY || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
            return;

        //드래그 종료
        UIManager.Instance.HideSelectingArrow();

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
        StartCoroutine(GameManager.Instance.PlayTask(Card.Battle(attackTarget.Card, ColumnIndex, attackTarget.ColumnIndex)));
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
        PlayerType owner = (PlacedObject == PlacedObjType.ALLY) ? PlayerType.ME : PlayerType.OPPONENT;
        UnregisterCard();
        moveTarget.RegisterCard(targetCard.gameObject, owner);
    }

    public virtual void OnPlaceEffect()
    {
        PlacedObjType foeObj = Card.Owner == PlayerType.ME ? PlacedObjType.ENEMY : PlacedObjType.ALLY;
        foreach (var tile in aroundTiles)
        {
            if (tile.PlacedObject == foeObj)
            {
                return;
            }
        }

        Card.BeExhausted();
    }

    public void RegisterCard(GameObject card, PlayerType owner)
    {
        if (PlacedObject != PlacedObjType.BLANK)
            return;

        Card = card.GetComponent<PlayingCard>();
        Card.deathEvent.AddListener(UnregisterCard);

        GameObject cardSpriteGO = card.FindChildGO(Functions.NAME__PLAYING_CARD__CARD_SPRITE);
        cardImage = cardSpriteGO.GetComponent<Image>();
        cardAnimator = cardSpriteGO.GetComponent<Animator>();
        cardDefaultMat = cardImage.material;

        PlacedObject = owner == PlayerType.ME ? PlacedObjType.ALLY : PlacedObjType.ENEMY;

        Field.AddTile(this, owner);
    }

    public void UnregisterCard()
    {
        Card.deathEvent.RemoveListener(UnregisterCard);

        Card = null;
        cardImage = null;
        cardAnimator = null;

        Field.RemoveTile(this,
            (PlacedObject == PlacedObjType.ALLY) ? PlayerType.ME : PlayerType.OPPONENT);

        PlacedObject = PlacedObjType.BLANK;
    }

    public void InitializeIndex(int tileRow, int tileCol)
    {
        if (RowIndex != -1 && ColumnIndex != -1)
            return;

        RowIndex = tileRow;
        ColumnIndex = tileCol;

        int row, col;
        for (int i = -1; i <= 1; i++)
        {
            row = RowIndex + i;
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                col = ColumnIndex + j;

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
                tileImage.color = COLOR_YELLOW;
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
}