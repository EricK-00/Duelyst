using UnityEngine;

public class SelectingArrow : MonoBehaviour
{
    public GameObject testGO;

    [SerializeField]
    private AnimationCurve curve;

    private Canvas objCanvas;
    private RectTransform objCanvasRect;

    private Vector3 mousePos;
    private Vector2 pointPos;
    private Vector2 lerpPointA, lerpPointB, pointInCurve;
    private Vector2 diffVec;

    private float lerpValue;
    private float angleValue;

    private void Awake()
    {
        objCanvas = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS).GetComponent<Canvas>();
        objCanvasRect = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS).GetComponent<RectTransform>();
    }

    void Update()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            {
                mousePos = Input.mousePosition / objCanvas.scaleFactor;
                mousePos = new Vector3(
                    mousePos.x - objCanvasRect.rect.width / 2, 
                    mousePos.y - objCanvasRect.rect.height / 2, 
                    mousePos.z);

                pointPos = new Vector2((mousePos.x + transform.localPosition.x) / 2, 0);

                testGO.transform.localPosition = pointPos;

                lerpValue = 1f / (childCount - 1) * i;

                lerpPointA = Vector2.Lerp(transform.localPosition, pointPos, lerpValue);
                lerpPointB = Vector2.Lerp(pointPos, mousePos, lerpValue);
                pointInCurve = Vector2.Lerp(lerpPointA, lerpPointB, lerpValue) - (Vector2)transform.localPosition;

                transform.GetChild(i).localPosition = pointInCurve;
                diffVec = mousePos - transform.localPosition;
                angleValue = Mathf.Atan2(diffVec.y, diffVec.x) * Mathf.Rad2Deg;

                transform.GetChild(i).rotation = Quaternion.Euler(0, 0, angleValue + 90);
            }
        }
    }
}