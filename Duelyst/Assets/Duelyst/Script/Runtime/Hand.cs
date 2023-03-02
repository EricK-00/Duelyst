using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EnumTypes;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform handRect;

    private GameObject cardSprite;
    private Sprite defaultCardSprite;
    private Animator cardAnim;

    private GameObject cardDetail;
    private Animator cardDetailAnim;
    private TMP_Text cardDetailPower;
    private TMP_Text cardDetailHealth;
    private TMP_Text cardDetailType;
    private TMP_Text cardDetailName;
    private TMP_Text cardDetailCost;
    private TMP_Text cardDetailDesc;

    private Animator drawAnim;

    private TMP_Text costText;

    private Card cardData;

    private bool isDragged = false;
    public bool NoCard { get; private set; } = true;

    private void Awake()
    {
        handRect = GetComponent<RectTransform>();

        cardSprite = gameObject.FindChildGO(Functions.NAME__HAND__CARD_SPRITE);
        defaultCardSprite = GetComponent<Image>().sprite;
        cardAnim = cardSprite.GetComponent<Animator>();

        cardDetail = gameObject.FindChildGO(Functions.NAME__CARD_DETAIL);
        cardDetailAnim = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__SPRITE).GetComponent<Animator>();
        cardDetailPower = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__POWER).GetComponent<TMP_Text>();
        cardDetailHealth = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__HEALTH).GetComponent<TMP_Text>();
        cardDetailType = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__TYPE).GetComponent<TMP_Text>();
        cardDetailName = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__NAME).GetComponent<TMP_Text>();
        cardDetailCost = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__COST).GetComponent<TMP_Text>();
        cardDetailDesc = cardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__DESC).GetComponent<TMP_Text>();

        drawAnim = gameObject.FindChildGO(Functions.NAME__HAND__DRAW_ANIM).GetComponent<Animator>();

        costText = gameObject.FindChildGO(Functions.NAME__HAND__COST_TEXT).GetComponent<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        if (NoCard)
            return;

        if (!isDragged)
        {
            ShowCardDetail();

            //idle로 전환
            cardAnim.SetBool("isTargetedInHands", true);
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
            cardAnim.SetBool("isTargetedInHands", false);
        }

        HideCardDetail();
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        if (NoCard || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT)
            return;

        isDragged = true;
    }

    public void OnDrag(PointerEventData ped)
    {
        /* Do nothing */
    }

    public void OnEndDrag(PointerEventData ped)
    {
        //드래그 종료
        UIManager.Instance.HideSelectingArrow();
        isDragged = false;

        //breathing으로 전환
        cardAnim.SetBool("isTargetedInHands", false);

        if (NoCard || GameManager.Instance.CurrentTurnPlayer == PlayerType.OPPONENT || GameManager.Instance.TaskBlock)
        {
            Field.HidePlaceableTiles();
            return;
        }

        //현재 레이캐스트 결과 가져오기
        GameObject raycastTarget = ped.pointerCurrentRaycast.gameObject;
        if (raycastTarget == null)
        {
            Field.HidePlaceableTiles();
            return;
        }

        Tile targetTile;
        ReplaceUI replaceUI;
        if (raycastTarget.TryGetComponent(out targetTile) && targetTile.IsPlaceable)
        {
            //코스트 지불
            if (!GameManager.Instance.TryCostMana(cardData.Cost, PlayerType.ME))
            {
                Field.HidePlaceableTiles();
                return;
            }

            //필드에 카드 생성
            PlacePlayingCard(targetTile);
        }
        else if (raycastTarget.TryGetComponent(out replaceUI))
        {
            ReplaceCard();
        }

        Field.HidePlaceableTiles();
    }

    private void PlacePlayingCard(Tile tile)
    {
        PlayingCardPoolingManager.Instance.ActiveNewCard(tile, cardData, PlayerType.ME);
        UIManager.Instance.PlayPlacingAnim(tile);
        tile.OnPlaceEffect();

        SetDefault();
    }

    private void ReplaceCard()
    {
        if (GameManager.Instance.ReplaceCard(cardData, out cardData, PlayerType.ME))
        {
            SetDefault();
            UIManager.Instance.AddCard(cardData);
        }
    }

    private void SetDefault()
    {
        NoCard = true;
        GetComponent<Image>().sprite = defaultCardSprite;
        cardAnim.runtimeAnimatorController = null;
        costText.text = string.Empty;
    }

    public void SetNewCard(Card card)
    {
        NoCard = false;
        cardData = card;
        cardAnim.runtimeAnimatorController = cardData.Anim;
        costText.SetTMPText(cardData.Cost);

        drawAnim.Play(Functions.NAME__ANIMATION_STATE__DRAW);
    }

    private void ShowCardDetail()
    {
        cardDetailAnim.runtimeAnimatorController = cardData.Anim;
        cardDetailPower.SetTMPText(cardData.Power);
        cardDetailHealth.SetTMPText(cardData.Health);
        cardDetailType.SetTMPText(cardData.Type);
        cardDetailName.SetTMPText(cardData.Name);
        cardDetailCost.SetTMPText(cardData.Cost);
        cardDetailDesc.SetTMPText(cardData.Description);

        if (cardData.Type != CardType.GENERAL)
            cardDetailCost.transform.parent.gameObject.SetActive(true);
        else
            cardDetailCost.transform.parent.gameObject.SetActive(false);

        cardDetail.SetActive(true);
    }

    private void HideCardDetail()
    {
        cardDetail.SetActive(false);
    }
}