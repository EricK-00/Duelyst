using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Place : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData ped)
    {
        image.fillCenter = false;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        image.fillCenter = true;
    }
}