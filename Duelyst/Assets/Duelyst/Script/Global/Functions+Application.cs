using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public static partial class Functions
{
    public static void QuitThisGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }       // QuitThisGame()

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }       // LoadScene()

    public static void ExtensionFunc(this GameObject go)
    {
        Debug.Log("Extension Method");
    }
}