using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Place : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private Image placeImage;
    private GameObject objCanvas;
    private RectTransform selectingArrowRect;

    private Material outline;

    [SerializeField]
    private GameObject cardGO;
    private Image cardImage;
    private Animator cardAnimator;

    private void Awake()
    {
        placeImage = GetComponent<Image>();
        objCanvas = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS);
        selectingArrowRect = objCanvas.FindChildGameObject(Functions.NAME_SELECTINGARROW).GetComponent<RectTransform>();

        outline = Functions.outline;
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        placeImage.fillCenter = false;
        if (cardGO == null)
            return;

        cardImage.material = outline;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        placeImage.fillCenter = true;
        if (cardGO == null)
            return;

        cardImage.material = null;
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        if (cardGO == null)
            return;

        //드래그 시작
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(transform.position));

        float CanvasZPos = objCanvas.transform.position.z;
        float distance = (CanvasZPos - ray.origin.z) / ray.direction.z;
        Vector3 point = ray.origin + ray.direction * distance;
        point.z = CanvasZPos;

        selectingArrowRect.transform.position = point;
        selectingArrowRect.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData ped)
    {

    }

    public void OnEndDrag(PointerEventData ped)
    {
        if (cardGO == null)
            return;

        //드래그 종료
        selectingArrowRect.gameObject.SetActive(false);

        //현재 레이캐스트 결과 가져오기
        GameObject raycastTarget = ped.pointerCurrentRaycast.gameObject;
        if (raycastTarget == null)
        {
            return;
        }

        if (raycastTarget.tag == Functions.TAG_ENEMY)
        {
            //공격
        }
        else if (raycastTarget.tag == Functions.TAG_PLACE)
        {
            //아무도 있지 않을 때
            //
            //

            //이동
            StartCoroutine(Move(raycastTarget));
        }
    }

    public void OnPointerClick(PointerEventData ped)
    {
        if (ped.button == PointerEventData.InputButton.Right)
        {
            //카드 상세보기
            UIManager.Instance.ShowPlayerCardDetail(cardAnimator);
        }
    }

    public IEnumerator Move(GameObject newPlace)
    {
        Vector3 destPos = newPlace.transform.GetComponent<RectTransform>().position;
        Vector3 sourcePos = transform.position;
        float timer = 1f;
        const int FRAME = 60;
        float term = (float)1f / FRAME;

        while (timer >= 0)
        {
            cardGO.transform.position = Vector3.Lerp(sourcePos, destPos, 1 - timer);

            yield return new WaitForSeconds(term);
            timer -= term;
        }

        //카드 등록 장소 이동
        newPlace.GetComponent<Place>().PlaceCard(cardGO);
        UnregisterCard();
    }

    public void PlaceCard(GameObject card)
    {
        if (cardGO != null)
        {
            Debug.Log("이미 놓인 장소");
            return;
        }

        RegisterCard(card);
    }

    private void RegisterCard(GameObject card)
    {
        cardGO = card;

        GameObject cardSpriteGO = card.FindChildGameObject(Functions.NAME_PLAYINGCARD_CARDSPRITE);
        cardImage = cardSpriteGO.GetComponent<Image>();
        cardAnimator = cardSpriteGO.GetComponent<Animator>();
        tag = Functions.TAG_UNTAGGED;
    }

    private void UnregisterCard()
    {
        cardGO = null;
        cardImage = null;
        cardAnimator = null;
        tag = Functions.TAG_PLACE;
    }
}