using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PlacedObj
{
    BLANK = 0,
    ALLY,
    ENEMY
}

public class Place : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private int rowIndex = -1;
    private int columnIndex = -1;
    [SerializeField]
    private List<Place> aroundPlaces = new List<Place>();
    [SerializeField]
    private List<Place> oneDistancePlaces = new List<Place>();

    private Image placeImage;
    private Color placeDefaultColor;
    private GameObject uiCanvas;
    private RectTransform selectingArrowRect;

    private GameObject cardGO;
    private Image cardImage;
    private Animator cardAnimator;

    private Material cardDefaultMat;
    private Material outline;

    public PlacedObj PlacedObject = PlacedObj.BLANK; //{ get; private set; } = PlacedObj.BLANK;

    private readonly Color movablePlaceColor = new Color(1, 1, 1, 0.1f); //white
    private readonly Color attackablePlaceColor = new Color(1, 1, 0, 0.3f);//yellow

    private void Awake()
    {
        placeImage = GetComponent<Image>();
        placeDefaultColor = placeImage.color;
        uiCanvas = Functions.GetRootGameObject(Functions.NAME_UICANVAS);
        selectingArrowRect = uiCanvas.FindChildGameObject(Functions.NAME_SELECTINGARROW).GetComponent<RectTransform>();

        outline = Functions.OUTLINE;
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        placeImage.fillCenter = false;
        if (PlacedObject == PlacedObj.BLANK)
            return;

        cardImage.material = outline;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        placeImage.fillCenter = true;
        if (PlacedObject == PlacedObj.BLANK)
            return;

        cardImage.material = cardDefaultMat;
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        if (PlacedObject == PlacedObj.BLANK)
            return;

        //드래그 시작
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(transform.position));

        float CanvasZPos = uiCanvas.transform.position.z;
        float distance = (CanvasZPos - ray.origin.z) / ray.direction.z;
        Vector3 point = ray.origin + ray.direction * distance;
        point.z = CanvasZPos;

        selectingArrowRect.transform.position = point;
        selectingArrowRect.gameObject.SetActive(true);

        ShowMoveRange();
        ShowAttackRange();
    }

    public void OnDrag(PointerEventData ped)
    {

    }

    public void OnEndDrag(PointerEventData ped)
    {
        if (PlacedObject == PlacedObj.BLANK)
            return;

        //드래그 종료
        selectingArrowRect.gameObject.SetActive(false);

        //현재 레이캐스트 결과 가져오기
        GameObject raycastTarget = ped.pointerCurrentRaycast.gameObject;
        if (raycastTarget == null)
        {
            HideAttackRange();
            HideMoveRange();

            return;
        }

        if (raycastTarget.tag == Functions.TAG_PLACE)
        {
            Color placeColor = raycastTarget.GetComponent<Image>().color;
            if (raycastTarget.GetComponent<Place>().PlacedObject == PlacedObj.ENEMY && placeColor == attackablePlaceColor)
            {
                Debug.Log("Battle");
            }
            else if (raycastTarget.GetComponent<Place>().PlacedObject == PlacedObj.BLANK && placeColor == movablePlaceColor)
            {
                if (GameManager.Instance.TaskBlock)
                    return;

                PlayingCard card = cardGO.GetComponent<PlayingCard>();
                Place newPlace = raycastTarget.GetComponent<Place>();

                //카드 등록 장소 변경
                newPlace.RegisterCard(card.gameObject);
                UnregisterCard();

                //이동
                StartCoroutine(GameManager.Instance.PlayTask(card.Move(raycastTarget, newPlace.GetRow())));
            }
        }

        HideAttackRange();
        HideMoveRange();
    }

    public int GetRow()
    {
        return rowIndex;
    }

    public void OnPointerClick(PointerEventData ped)
    {
        if (PlacedObject == PlacedObj.BLANK)
        {
            return;
        }

        if (ped.button == PointerEventData.InputButton.Right)
        {
            //카드 상세보기
            UIManager.Instance.ShowPlayerCardDetail(cardAnimator);
        }
    }

    public void RegisterCard(GameObject card)
    {
        if (PlacedObject == PlacedObj.ENEMY)
        {
            return;
        }

        cardGO = card;

        GameObject cardSpriteGO = card.FindChildGameObject(Functions.NAME_PLAYINGCARD_CARDSPRITE);
        cardImage = cardSpriteGO.GetComponent<Image>();
        cardAnimator = cardSpriteGO.GetComponent<Animator>();
        cardDefaultMat = cardImage.material;

        PlacedObject = PlacedObj.ALLY;
    }

    private void UnregisterCard()
    {
        cardGO = null;
        cardImage = null;
        cardAnimator = null;

        PlacedObject = PlacedObj.BLANK;
    }

    public void InitializeIndex(int placeRow, int placeCol)
    {
        if (rowIndex != -1 && columnIndex != -1)
            return;

        rowIndex = placeRow;
        columnIndex = placeCol;

        int row, col;
        for (int i = -1; i <= 1; i++)
        {
            row = rowIndex + i;
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                col = columnIndex + j;

                Place place;
                if (Board.TryGetPlace(row, col, out place))
                {
                    aroundPlaces.Add(place);

                    if (Mathf.Abs(i) + Mathf.Abs(j) == 1)
                    {
                        oneDistancePlaces.Add(place);
                    }
                }
            }
        }
    }

    public void ShowAttackRange()
    {
        Color color = attackablePlaceColor;

        foreach (var place in aroundPlaces)
        {
            if (place.PlacedObject == PlacedObj.ENEMY)
                place.ActivePlace(color);
        }
    }

    public void HideAttackRange()
    {
        foreach (var place in aroundPlaces)
        {
            if (place.PlacedObject == PlacedObj.ENEMY)
                place.InactivePlace();
        }
    }

    public void ShowMoveRange()
    {
        ShowOneDistanceRange();
        foreach (var place in oneDistancePlaces)
        {
            if (place.PlacedObject != PlacedObj.ENEMY)
                place.ShowOneDistanceRange();
        }
    }

    public void HideMoveRange()
    {
        HideOneDistanceRange();
        foreach (var place in oneDistancePlaces)
        {
            if (place.PlacedObject != PlacedObj.ENEMY)
                place.HideOneDistanceRange();
        }
    }

    public void ShowOneDistanceRange()
    {
        Color color = movablePlaceColor;

        foreach (var place in oneDistancePlaces)
        {
            if (place.PlacedObject == PlacedObj.BLANK)
                place.ActivePlace(color);
        }
    }

    public void HideOneDistanceRange()
    {
        foreach (var place in oneDistancePlaces)
        {
            place.InactivePlace();
        }
    }

    public void ActivePlace(Color color)
    {
        placeImage.color = color;
    }

    public void InactivePlace()
    {
        placeImage.color = placeDefaultColor;
    }
}