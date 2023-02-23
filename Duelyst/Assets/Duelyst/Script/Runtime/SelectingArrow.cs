using UnityEngine;

public class SelectingArrow : MonoBehaviour
{
    private const int MIN_ARROW_COUNT = 2;//2 이상

    public GameObject testGO;
    public GameObject target;

    private Canvas uiCanvas;
    private RectTransform uiCanvasRect;

    private Vector3 mousePos;
    private Vector2 pointPos;
    private Vector2 lerpPointA, lerpPointB, pointInCurve;
    private Vector2 direcVec;

    private float lerpValue;
    private float angleValue;

    private void Awake()
    {
        uiCanvas = Functions.GetRootGO(Functions.NAME_UICANVAS).GetComponent<Canvas>();
        uiCanvasRect = uiCanvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        mousePos = Input.mousePosition / uiCanvas.scaleFactor;
        mousePos = new Vector3(
            mousePos.x - uiCanvasRect.rect.width / 2,
            mousePos.y - uiCanvasRect.rect.height / 2,
            mousePos.z);

        pointPos = new Vector2((mousePos.x + transform.localPosition.x) / 2,
            100 + (mousePos.y + transform.localPosition.y) / 2);

        testGO.transform.localPosition = pointPos - (Vector2)transform.localPosition;

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

                lerpValue = 1f / (arrowCount - 1) * i;//arrowCount가 2이상

                lerpPointA = Vector2.Lerp(transform.localPosition, pointPos, lerpValue);
                lerpPointB = Vector2.Lerp(pointPos, mousePos, lerpValue);
                pointInCurve = Vector2.Lerp(lerpPointA, lerpPointB, lerpValue) - (Vector2)transform.localPosition;

                arrow.SetActive(true);
                arrow.transform.localPosition = pointInCurve;
                direcVec = mousePos - transform.localPosition;
                angleValue = Mathf.Atan2(direcVec.y, direcVec.x) * Mathf.Rad2Deg;

                arrow.transform.rotation = Quaternion.Euler(0, 0, angleValue);
            }
        }
        target.transform.localPosition = (Vector2)mousePos - (Vector2)transform.localPosition;
        target.SetActive(true);
        for (int i = arrowCount; i < transform.childCount - 2; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}