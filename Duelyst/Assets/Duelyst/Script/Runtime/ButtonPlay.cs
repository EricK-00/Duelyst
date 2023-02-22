using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPlay : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData ped)
    {
        //Functions.LoadScene("InGameScene");
        Functions.LoadScene("InGameDev");
    }
}