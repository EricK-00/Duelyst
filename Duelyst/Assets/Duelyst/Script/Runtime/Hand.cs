using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform selectingArrowRect;
    private RectTransform handRect;

    private GameObject objCanvas;
    private GameObject uiCanvas;
    private GameObject cardDetail;
    private GameObject card;
    private Image cardImage;
    private Sprite defaultCardSprite;
    private Animator cardAnimator;

    private Animator drawAnimator;

    private TMP_Text costText;

    private bool isDragged = false;
    public bool NoCard { get; private set; } = true;

    private void Awake()
    {
        objCanvas = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS);
        uiCanvas = Functions.GetRootGameObject(Functions.NAME_UICANVAS);
        selectingArrowRect = uiCanvas.FindChildGameObject(Functions.NAME_SELECTINGARROW).GetComponent<RectTransform>();
        handRect = GetComponent<RectTransform>();

        cardDetail = gameObject.FindChildGameObject(Functions.NAME_HAND_CARDDETAIL);
        card = gameObject.FindChildGameObject(Functions.NAME_HAND_CARDSPRITE);
        cardImage = GetComponent<Image>();
        defaultCardSprite = cardImage.sprite;
        cardAnimator = card.GetComponent<Animator>();

        drawAnimator = gameObject.FindChildGameObject(Functions.NAME_HAND_DRAWANIM).GetComponent<Animator>();

        costText = gameObject.FindChildGameObject(Functions.NAME_HAND_COSTTEXT).GetComponent<TMP_Text>();
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
            selectingArrowRect.gameObject.SetActive(true);
            selectingArrowRect.position = handRect.position;
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
        if (NoCard)
            return;

        isDragged = true;
    }

    public void OnDrag(PointerEventData ped)
    {
        //OnBeginDrag()와 OnEndDrag()에 이벤트를 받기 위해 필요
        /* Do nothing */
    }

    public void OnEndDrag(PointerEventData ped)
    {
        if (NoCard)
            return;

        //드래그 종료
        selectingArrowRect.gameObject.SetActive(false);
        isDragged = false;

        //breathing으로 전환
        cardAnimator.SetBool("isMouseOver", false);

        //현재 레이캐스트 결과 가져오기
        GameObject raycastTarget = ped.pointerCurrentRaycast.gameObject;
        if (raycastTarget == null)
        {
            return;
        }

        if (raycastTarget.CompareTag(Functions.TAG_PLACE) && raycastTarget.GetComponent<Place>().PlacedObject == PlacedObj.BLANK)
        {
            //필드에 카드 생성
            PlacePlayingCard(raycastTarget.gameObject);
            SetDefault();
        }
    }

    private void PlacePlayingCard(GameObject place)
    {
        GameObject playingCard = Instantiate(Functions.PLAYINGCARD, objCanvas.transform);
        playingCard.transform.position = place.transform.position;

        GameObject cardSprite = playingCard.FindChildGameObject(Functions.NAME_PLAYINGCARD_CARDSPRITE);

        cardSprite.GetComponent<Image>().sprite = cardImage.sprite;
        cardSprite.GetComponent<Animator>().runtimeAnimatorController = cardAnimator.runtimeAnimatorController;
        cardSprite.GetComponent<Animator>().SetBool("onField", true);

        place.GetComponent<Place>().RegisterCard(playingCard);

        GameManager.Instance.ReduceHandsCount();
    }

    private void SetDefault()
    {
        NoCard = true;
        cardImage.sprite = defaultCardSprite;
        cardAnimator.runtimeAnimatorController = null;
        costText.text = string.Empty;
    }

    public void SetNewCard(GameObject cardGO, int cost)
    {
        NoCard = false;
        cardImage.sprite = cardGO.GetComponent<Image>().sprite;
        cardAnimator.runtimeAnimatorController = cardGO.GetComponent<Animator>().runtimeAnimatorController;
        costText.text = cost.ToString();

        drawAnimator.Play("Hand_Draw");
    }
}