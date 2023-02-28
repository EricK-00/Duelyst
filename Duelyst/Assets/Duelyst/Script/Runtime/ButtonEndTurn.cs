using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEndTurn : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData ped)
    {
        GameManager.Instance.EndMyTurn();
    }
}
