using UnityEngine;

public class SelectingArrow : MonoBehaviour
{
    private const int MIN_ARROW_COUNT = 2;//2 이상

    private Canvas uiCanvas;
    private RectTransform uiCanvasRect;
    private GameObject mousePointImage;

    private void Awake()
    {
        uiCanvas = Functions.GetRootGO(Functions.NAME__UI_CANVAS).GetComponent<Canvas>();
        uiCanvasRect = uiCanvas.GetComponent<RectTransform>();
        mousePointImage = transform.GetChild(transform.childCount - 1).gameObject;
    }

    private void OnEnable()
    {
        SetArrowTransform();
    }

    void Update()
    {
        SetArrowTransform();
    }

    private void SetArrowTransform()
    {
        Vector3 mousePos = Input.mousePosition / uiCanvas.scaleFactor;
        mousePos = new Vector3(
            mousePos.x - uiCanvasRect.rect.width / 2, 
            mousePos.y - uiCanvasRect.rect.height / 2,
            mousePos.z);

        Vector2 pointPos = new Vector2((mousePos.x + transform.localPosition.x) / 2,
            100 + (mousePos.y + transform.localPosition.y) / 2);

        int arrowCount = MIN_ARROW_COUNT + Mathf.Max((int)Mathf.Abs(mousePos.x - transform.localPosition.x) / 50, (int)Mathf.Abs(mousePos.y - transform.localPosition.y) / 50);

        arrowCount = Mathf.Min(transform.childCount - 1, arrowCount);
        if (arrowCount < 2)
        {
            return;
        }
        for (int i = 0; i < arrowCount; i++)
        {
            {
                GameObject arrow = transform.GetChild(i).gameObject;

                float lerpValue = 1f / (arrowCount - 1) * i;//arrowCount가 2 이상(1일 때 zero division)

                Vector2 lerpPointA = Vector2.Lerp(transform.localPosition, pointPos, lerpValue);
                Vector2 lerpPointB = Vector2.Lerp(pointPos, mousePos, lerpValue);
                Vector2 pointInCurve = Vector2.Lerp(lerpPointA, lerpPointB, lerpValue) - (Vector2)transform.localPosition;

                arrow.SetActive(true);
                arrow.transform.localPosition = pointInCurve;

                //마우스 쪽으로 방향 전환
                Vector2 direcVec = mousePos - transform.localPosition;
                float degree = Mathf.Atan2(direcVec.y, direcVec.x) * Mathf.Rad2Deg;
                arrow.transform.rotation = Quaternion.Euler(0, 0, degree);
            }
        }
        mousePointImage.transform.localPosition = (Vector2)mousePos - (Vector2)transform.localPosition;
        mousePointImage.SetActive(true);
        for (int i = arrowCount; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}