using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlay : MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        Functions.LoadScene("InGameScene");
    }
}