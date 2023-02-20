using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawTest : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData ped)
    {
        GameManager.Instance.DrawCard();
    }
}