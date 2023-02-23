using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public static partial class Functions
{
    public static void SetTMPText(this TMP_Text tmp, string newText)
    {
        tmp.text = newText;
    }

    public static void SetTMPText<T>(this TMP_Text tmp, T newText) where T : unmanaged
    {
        tmp.text = newText.ToString();
    }
}