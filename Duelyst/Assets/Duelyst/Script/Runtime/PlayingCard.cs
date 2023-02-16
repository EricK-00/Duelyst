using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayingCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Material outline;

    private void Awake()
    {
        outline = Functions.outline;
    }

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        Debug.Log("aaa");
        image.material = outline;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        Debug.Log("bbb");
        image.material = null;
    }
}