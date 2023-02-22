using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayingCard : MonoBehaviour
{
    public IEnumerator Move(GameObject newPlace, int row)
    {
        SetLayer(row);

        const int FRAME = 60;

        Vector3 destPos = newPlace.transform.GetComponent<RectTransform>().position;
        Vector3 sourcePos = transform.position;
        float timer = 1f;
        float term = (float)1f / FRAME;

        while (timer >= 0)
        {
            transform.position = Vector3.Lerp(sourcePos, destPos, 1 - timer);

            yield return new WaitForSeconds(term);
            timer -= term;
        }
    }

    public void SetLayer(int layerNum)
    {
        transform.SetParent(GameManager.Instance.Layers[layerNum]);
    }
}