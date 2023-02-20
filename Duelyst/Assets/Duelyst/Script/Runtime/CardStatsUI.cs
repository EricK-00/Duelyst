using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class CardStatsUI : MonoBehaviour
{
    Image image;
    TMP_Text text;

    private void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponent<TMP_Text>();

        if (image != null )
        image.material.renderQueue = (int)RenderQueue.Transparent;

        //if (text != null)
        //text.fontMaterial.renderQueue = (int)RenderQueue.Overlay;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}