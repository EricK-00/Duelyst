using EnumTypes;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReplaceUI : MonoBehaviour, IPointerEnterHandler,  IPointerExitHandler
{
    private const float ROTATION_SPEED = 7.0f;

    private GameObject outerRing;
    private GameObject innerGlow;
    private GameObject innerRing;

    private void Awake()
    {
        outerRing = gameObject.FindChildGO("ReplaceOuterRing");
        innerGlow = gameObject.FindChildGO("ReplaceInnerGlow");
        innerRing = gameObject.FindChildGO("ReplaceInnerRing");
    }

    private void Start()
    {
        StartCoroutine(RotateRing());
    }

    private IEnumerator RotateRing()
    {
        int frame = 100;
        float term = 1f / frame;

        while (true)
        {
            outerRing.transform.Rotate(0, 0, -1 * ROTATION_SPEED * term);
            innerRing.transform.Rotate(0, 0, +1 * ROTATION_SPEED * term);

            yield return new WaitForSeconds(term);
        }
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        innerGlow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        innerGlow.SetActive(false);
    }

    public void ReplaceCard()
    {

    }
}