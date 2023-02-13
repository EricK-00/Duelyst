using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public static partial class Functions
{
    public static void SetTextMeshPro(GameObject go, string newText)
    {
        TMP_Text tmpText = go.GetComponent<TMP_Text>();
        if (tmpText == null)
            return;

        tmpText.text = newText;

    }       //SetTextMeshPro()
}