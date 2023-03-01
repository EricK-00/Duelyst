using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonGoMainMenu : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData ped)
    {
        if (ped.button == PointerEventData.InputButton.Left)
        {
            Functions.LoadScene("01.MainMenuScene");
            Time.timeScale = 1.0f;
        }
    }
}
