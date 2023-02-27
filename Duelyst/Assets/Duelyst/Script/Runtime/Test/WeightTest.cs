using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeightTest : MonoBehaviour
{
    TMP_Text weightText;

    private void Awake()
    {
        weightText = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        weightText.SetTMPText(transform.parent.GetComponent<Tile>().debug_Weight);
    }
}