using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    float maxTime = 3f;
    float startMaxTime = 3f;

    public RectTransform rect1;
    public RectTransform rect2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(rect1.position, rect2.position, maxTime / startMaxTime);

        maxTime -= Time.deltaTime;
        if (maxTime <= 0)
        {


            maxTime = 3f;
        }
    }
}