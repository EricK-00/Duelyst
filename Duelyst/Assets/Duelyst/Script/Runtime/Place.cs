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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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