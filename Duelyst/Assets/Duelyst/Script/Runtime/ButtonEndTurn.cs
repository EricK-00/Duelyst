using UnityEngine;
using UnityEngine.EventSystems;
using EnumTypes;

public class ButtonEndTurn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData ped)
    {
        if (GameManager.Instance.CurrentTurnPlayer == PlayerType.ME && ped.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.EndMyTurn();
        }
    }
}