using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class Functions
{
    //특정 게임오브젝트의 자식을 찾는 메서드
    public static GameObject FindChildGO(this GameObject parentGO, string childName)
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
                GameObject go = FindChildGO(searchTarget, childName);
                if (go != null)
                    return go;
            }
        }

        return null;
    }

    //루트 게임오브젝트를 찾는 메서드
    public static GameObject GetRootGO(string goName)
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

        return null;
    }

    public static Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }
}