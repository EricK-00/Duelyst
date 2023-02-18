using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayingCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private GameObject objCanvas;
    private RectTransform selectingArrowRect;

    private Material outline;
    private Image image;
    private Animator animator;

    private void Awake()
    {
        objCanvas = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS);
        selectingArrowRect = objCanvas.FindChildGameObject(Functions.NAME_SELECTINGARROW).GetComponent<RectTransform>();

        outline = Functions.outline;
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        image.material = outline;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        image.material = null;
    }

    public void OnBeginDrag(PointerEventData ped)
    {
        //드래그 시작
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(transform.position));

        float CanvasZPos = objCanvas.transform.position.z;
        float distance = (CanvasZPos - ray.origin.z) / ray.direction.z;
        Vector3 point = ray.origin + ray.direction * distance;
        point.z = CanvasZPos;

        selectingArrowRect.transform.position = point;
        selectingArrowRect.gameObject.SetActive(true);

        Debug.Log($"{ray.origin} {ray.direction}");
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
                //null 체크
                StartCoroutine(Move(raycastTarget.transform.GetChild(0).GetComponent<RectTransform>()));
            }
        }

        //드래그 종료
        selectingArrowRect.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData ped)
    {
        if (ped.button == PointerEventData.InputButton.Right)
        {
            //카드 상세보기
            UIManager.Instance.ShowPlayerCardDetail(image.sprite, animator.runtimeAnimatorController);
        }
    }

    public IEnumerator Move(RectTransform destRect)
    {
        Vector3 sourcePos = transform.position;
        float timer = 1f;
        const int FRAME = 60;
        float term = (float)1f / FRAME;

        transform.SetParent(objCanvas.transform);

        while (timer >= 0)
        {
            transform.position = Vector3.Lerp(sourcePos, destRect.position, 1 - timer);

            yield return new WaitForSeconds(term);
            timer -= term;
        }

        //
    }
}