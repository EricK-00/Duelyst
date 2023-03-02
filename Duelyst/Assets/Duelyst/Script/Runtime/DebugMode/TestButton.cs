using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestButton : MonoBehaviour, IPointerUpHandler
{

    public void OnPointerUp(PointerEventData ped)
    {
        Functions.LoadScene("01.MainMenuScene");
        //StartCoroutine(GameManager.Instance.PlayTask(GameManager.Instance.DrawCard()));
    }
}