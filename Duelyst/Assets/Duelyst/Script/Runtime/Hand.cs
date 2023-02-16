using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private RectTransform selectingArrowRect;
    private RectTransform handRect;

    private GameObject cardDetail;
    private GameObject card;

    private bool draggingThis = false;

    private void Awake()
    {
        selectingArrowRect = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS).FindChildGameObject(Functions.NAME_SELECTINGARROW).GetComponent<RectTransform>();
        handRect = GetComponent<RectTransform>();

        cardDetail = gameObject.FindChildGameObject(Functions.NAME_HAND_CARDDETAIL);
        card = gameObject.FindChildGameObject(Functions.NAME_HAND_CARD);
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        if (!draggingThis)
        {
        //카드 상세보기 + idle로 전환
        cardDetail.SetActive(true);
        card.GetComponent<Animator>()?.SetBool("isMouseOver", true);
        }
    }

    public void OnPointerExit(PointerEventData ped)
    {
        if (draggingThis)
        {
            //드래그 시작
            selectingArrowRect.gameObject.SetActive(true);
            selectingArrowRect.position = handRect.position;
        }
        else
        {
            //breathing으로 전환
            card.GetComponent<Animator>()?.SetBool("isMouseOver", false);
        }

        //카드 상세보기 삭제
        cardDetail.SetActive(false);
    }

    public void OnDrag(PointerEventData ped)
    {

    }

    public void OnEndDrag(PointerEventData ped)
    {
        //레이캐스트로 타겟 가져오기
        GameObject raycastTarget = ped.pointerCurrentRaycast.gameObject;
        if (raycastTarget != null)
        {
            Debug.Log(raycastTarget.tag);
            if (raycastTarget.tag == Functions.TAG_PLACE)
            {
                GameObject playingCard = raycastTarget.transform.GetChild(0).gameObject;

                playingCard.GetComponent<Image>().sprite = card.GetComponent<Image>().sprite;
                playingCard.GetComponent<Animator>().runtimeAnimatorController = card.GetComponent<Animator>().runtimeAnimatorController;
                playingCard.SetActive(true);

                //GameObject go = card.gameObject;
                //go.transform.parent = ped.pointerCurrentRaycast.gameObject.transform;
                //go.transform.localPosition = Vector3.zero;
            }
        }

        //드래그 종료 + breathing으로 전환
        draggingThis = false;
        selectingArrowRect.gameObject.SetActive(false);
        card.GetComponent<Animator>()?.SetBool("isMouseOver", false);
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        draggingThis = true;
    }
}