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

    private int rowIndex = -1;
    private int columnIndex = -1;
    [SerializeField]
    private List<Tile> aroundTiles = new List<Tile>();
    [SerializeField]
    private List<Tile> oneDistanceTiles = new List<Tile>();

    private RectTransform tileRect;

    private Image tileImage;
    private Color tileDefaultColor;

    private PlayingCard playingCard;
    private Image cardImage;
    private Animator cardAnimator;

    private Material cardDefaultMat;
    private Material outline;

    public PlacedObjType PlacedObject = PlacedObjType.BLANK; //{ get; private set; } = PlacedObj.BLANK;

    public bool IsPlaceable { get; private set; } = false;
    public bool isMovable { get; private set; } = false;
    public bool isAttackable { get; private set; } = false;

    private void Awake()
    {
        tileRect = GetComponent<RectTransform>();

        tileImage = GetComponent<Image>();
        tileDefaultColor = tileImage.color;

        outline = Functions.OUTLINE;
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        tileImage.fillCenter = false;
        if (PlacedObject == PlacedObjType.BLANK)
            return;

        //카드 상세보기
        UIManager.Instance.ShowPlayerCardDetail(cardAnimator);

        cardImage.material = outline;
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
        if (PlacedObject == PlacedObjType.BLANK || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
            return;

        //드래그 시작
        UIManager.Instance.ShowSelectingArrow(tileRect);

        if (playingCard != null && playingCard.MoveChance > 0)
            ShowMoveRange();

        if (playingCard != null && playingCard.AttackChance > 0)
            ShowAttackRange();
    }

    public void OnDrag(PointerEventData ped)
    {

    }

    public void OnEndDrag(PointerEventData ped)
    {
        if (PlacedObject == PlacedObjType.BLANK || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
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
            Battle(targetTile);
        }
        else if (targetTile.isMovable)
        {
            Move(targetTile);
        }

        HideAttackRange();
        HideMoveRange();
    }

    private void Battle(Tile attackTarget)
    {
        StartCoroutine(GameManager.Instance.PlayTask(playingCard.Battle(attackTarget.playingCard, columnIndex, attackTarget.columnIndex)));
    }

    private void Move(Tile moveTarget)
    {
        PlayingCard targetCard = playingCard.GetComponent<PlayingCard>();

        //카드 등록 장소 변경
        moveTarget.RegisterCard(targetCard.gameObject);
        UnregisterCard();

        //이동
        StartCoroutine(GameManager.Instance.PlayTask(targetCard.Move(moveTarget, moveTarget.GetRow(), columnIndex, moveTarget.columnIndex)));
    }

    public int GetRow()
    {
        return rowIndex;
    }

    public virtual void OnPlaceEffect()
    {
        //Special tiles override this
    }

    public void RegisterCard(GameObject card)
    {
        if (PlacedObject != PlacedObjType.BLANK)
            return;

        playingCard = card.GetComponent<PlayingCard>();
        playingCard.deathEvent.AddListener(UnregisterCard);

        GameObject cardSpriteGO = card.FindChildGO(Functions.NAME_PLAYINGCARD_CARDSPRITE);
        cardImage = cardSpriteGO.GetComponent<Image>();
        cardAnimator = cardSpriteGO.GetComponent<Animator>();
        cardDefaultMat = cardImage.material;

        PlacedObject = PlacedObjType.ALLY;

        Field.RegisterTile(this);
    }

    public void UnregisterCard()
    {
        playingCard.deathEvent.RemoveListener(UnregisterCard);

        playingCard = null;
        cardImage = null;
        cardAnimator = null;

        PlacedObject = PlacedObjType.BLANK;

        Field.UnregisterTile(this);
    }

    public void InitializeIndex(int tileRow, int tileCol)
    {
        if (rowIndex != -1 && columnIndex != -1)
            return;

        rowIndex = tileRow;
        columnIndex = tileCol;

        int row, col;
        for (int i = -1; i <= 1; i++)
        {
            row = rowIndex + i;
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                col = columnIndex + j;

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