using UnityEngine;

public class ReplaceRing : MonoBehaviour
{
    private const float ROTATION_SPEED = 5.0f;

    void Update()
    {
        transform.Rotate(0, 0, -1 * ROTATION_SPEED * Time.deltaTime);
    }
}