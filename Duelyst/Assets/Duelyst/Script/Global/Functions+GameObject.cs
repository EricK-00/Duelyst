using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class Functions
{
    //! 특정 게임오브젝트의 자식을 탐색해서 찾는 메서드
    //private static GameObject GetChildGameObject(this GameObject parentGO, string childName)
    //{
    //    for (int i = 0; i < parentGO.transform.childCount; i++)
    //    {
    //        if (parentGO.transform.GetChild(i).name.Equals(childName))
    //        {
    //            return parentGO.transform.GetChild(i).gameObject;
    //        }
    //    }

    //    return default;
    //}

    //! 특정 게임오브젝트의 자식들을 탐색해서 찾는 메서드
    public static GameObject FindChildGameObject(this GameObject parentGO, string childName)
    {
        GameObject searchTarget;

        for (int i = 0; i < parentGO.transform.childCount; i++)
        {
            searchTarget = parentGO.transform.GetChild(i).gameObject;
            if (searchTarget.name.Equals(childName))
            {
                return searchTarget;
            }
            else
            {
                GameObject go = FindChildGameObject(searchTarget, childName);
                if (go != null)
                    return go;
            }
        }

        return null;
    }

    //! 루트 게임오브젝트를 탐색해서 찾는 메서드
    public static GameObject GetRootGameObject(string goName)
    {
        Scene activeScene = GetActiveScene();
        GameObject[] rootGOs = activeScene.GetRootGameObjects();

        foreach (GameObject go in rootGOs)
        {
            if (go.name.Equals(goName))
            {
                return go;
            }
        }

        return default;
    }

    public static Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

    public static void LookAtVector2(this Transform transform, Transform targetTransform, Vector2 forwardDirection)
    {
        Vector2 directionVector = (targetTransform.position - transform.position).normalized;
        bool isLeft = Vector2.Dot(directionVector, Vector2.right) >= 0;
        float angle = Vector2.Angle(forwardDirection, directionVector);

        transform.rotation = isLeft ? Quaternion.Euler(0, 0, -angle) : Quaternion.Euler(0, 0, angle);
    }
}