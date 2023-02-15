using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEndTurn : MonoBehaviour
{
    public void OnEndTurnButtonClick()
    {
        GameManager.Instance.EndTurn();
    }
}
