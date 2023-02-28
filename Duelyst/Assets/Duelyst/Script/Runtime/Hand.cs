using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EnumTypes;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform handRect;

    private GameObject objCanvas;
    private GameObject cardDetail;
    private GameObject card;
    private Image cardImage;
    private Sprite defaultCardSprite;
    private Animator cardAnimator;

    private Animator drawAnimator;

    private TMP_Text costText;

    private Card cardData;

    private bool isDragged = false;
    public bool NoCard { get; private set; } = true;

    private void Awake()
    {
        handRect = GetComponent<RectTransform>();

        objCanvas = Functions.GetRootGO(Functions.NAME__OBJ_CANVAS);

        cardDetail = gameObject.FindChildGO(Functions.NAME__HAND__CARD_DETAIL);
        card = gameObject.FindChildGO(Functions.NAME__HAND__CARD_SPRITE);
        cardImage = GetComponent<Image>();
        defaultCardSprite = cardImage.sprite;
        cardAnimator = card.GetComponent<Animator>();

        drawAnimator = gameObject.FindChildGO(Functions.NAME__HAND__DRAW_ANIM).GetComponent<Animator>();

        costText = gameObject.FindChildGO(Functions.NAME__HAND__COST_TEXT).GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        if (NoCard)
            return;

        if (!isDragged)
        {
            //카드 상세보기 + idle로 전환
            cardDetail.SetActive(true);
            cardAnimator.SetBool("isMouseOver", true);
        }
    }

    public void OnPointerExit(PointerEventData ped)
    {
        if (NoCard)
            return;

        if (isDragged)
        {
            //드래그 시작
            UIManager.Instance.ShowSelectingArrow(handRect);
            Field.ShowPlaceableTiles();
        }
        else
        {
            //breathing으로 전환
            cardAnimator.SetBool("isMouseOver", false);
        }

        //카드 상세보기 삭제
        cardDetail.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        if (NoCard || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
            return;

        isDragged = true;
    }

    public void OnDrag(PointerEventData ped)
    {
        //OnBeginDrag()와 OnEndDrag()에 이벤트 데이터를 받기 위해 필요
        /* Do nothing */
    }

    public void OnEndDrag(PointerEventData ped)
    {
        if (NoCard || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
            return;

        //드래그 종료
        UIManager.Instance.HideSelectingArrow();
        isDragged = false;

        //breathing으로 전환
        cardAnimator.SetBool("isMouseOver", false);

        //현재 레이캐스트 결과 가져오기
        GameObject raycastTarget = ped.pointerCurrentRaycast.gameObject;
        if (raycastTarget == null)
        {
            Field.HidePlaceableTiles();
            return;
        }

        Tile targetTile;
        if (raycastTarget.TryGetComponent<Tile>(out targetTile) && targetTile.IsPlaceable)
        {
            //코스트 지불
            if (!GameManager.Instance.TryCostMana(cardData.Cost, PlayerType.ME))
            {
                Field.HidePlaceableTiles();
                return;
            }

            //필드에 카드 생성
            PlacePlayingCard(targetTile);
            SetDefault();
        }

        Field.HidePlaceableTiles();
    }

    private void PlacePlayingCard(Tile tile)
    {
        PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(tile, cardData, false, PlayerType.ME);
        tile.OnPlaceEffect();
    }

    private void SetDefault()
    {
        NoCard = true;
        cardImage.sprite = defaultCardSprite;
        cardAnimator.runtimeAnimatorController = null;
        costText.text = string.Empty;
    }

    public void SetNewCard(Card card)
    {
        NoCard = false;

        cardData = card;
        cardAnimator.runtimeAnimatorController = cardData.Anim;
        costText.SetTMPText(cardData.Cost);

        drawAnimator.Play("HandDraw_Start");
    }
}