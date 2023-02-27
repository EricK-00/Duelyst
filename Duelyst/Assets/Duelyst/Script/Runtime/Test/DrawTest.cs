using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawTest : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData ped)
    {
        Functions.LoadScene("MainScene");
        //StartCoroutine(GameManager.Instance.PlayTask(GameManager.Instance.DrawCard()));
    }
}