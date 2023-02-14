using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{

    public RectTransform selectingArrowRect;
    private RectTransform handRect;

    private void Awake()
    {
        handRect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        if (ped.dragging)
        {
            //드래그 종료
            selectingArrowRect.gameObject.SetActive(false);
        }

        //카드 상세보기 + idle로 전환;
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(1).gameObject.GetComponent<Animator>()?.SetBool("isMouseOver", true);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        if (ped.dragging)
        {
            //드래그 시작
            selectingArrowRect.gameObject.SetActive(true);
            selectingArrowRect.position = handRect.position;
        }

        //카드 상세보기 삭제 + breathing으로 전환;
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(1).gameObject.GetComponent<Animator>()?.SetBool("isMouseOver", false);
    }

    public void OnDrag(PointerEventData ped)
    {

    }

    public void OnEndDrag(PointerEventData ped)
    {
        //레이캐스트로 타겟 가져오기
        Debug.Log(ped.pointerCurrentRaycast.gameObject.tag);

        //드래그 종료
        selectingArrowRect.gameObject.SetActive(false);
    }
}