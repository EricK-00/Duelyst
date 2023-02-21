using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayingCard : MonoBehaviour
{
    public IEnumerator Move(GameObject newPlace)
    {
        Vector3 destPos = newPlace.transform.GetComponent<RectTransform>().position;
        Vector3 sourcePos = transform.position;
        float timer = 1f;
        const int FRAME = 60;
        float term = (float)1f / FRAME;

        while (timer >= 0)
        {
            transform.position = Vector3.Lerp(sourcePos, destPos, 1 - timer);

            yield return new WaitForSeconds(term);
            timer -= term;
        }
    }
}