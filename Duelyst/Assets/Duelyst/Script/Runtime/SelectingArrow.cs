using UnityEngine;

public class SelectingArrow : MonoBehaviour
{
    private const float Z_POS_MAX = -500;

    [SerializeField]
    private AnimationCurve curve;

    private Vector3 mousePos;
    private float t;
    private float zPos;

    void Update()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            {
                mousePos = Input.mousePosition / transform.parent.GetComponent<Canvas>().scaleFactor;
                mousePos = new Vector3(mousePos.x - Screen.width / 2, mousePos.y - Screen.height / 2, transform.parent.GetComponent<Canvas>().transform.position.z);
                Debug.Log(mousePos);
                Debug.Log(transform.parent.GetComponent<Canvas>().scaleFactor);
                //mousePos.z = 90;
                t = 1f / (childCount - 1) * i;
                transform.GetChild(i).localPosition = Vector2.Lerp(transform.localPosition, mousePos, t) - (Vector2)transform.localPosition;
                zPos = Z_POS_MAX * curve.Evaluate(t);
                transform.GetChild(i).localPosition = new Vector3(transform.GetChild(i).localPosition.x, transform.GetChild(i).localPosition.y, zPos);
            }
        }
    }
}