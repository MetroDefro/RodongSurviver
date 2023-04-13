using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RodongSurviver.Base;

public class SceneManager
{
    public SceneBase CurrentScene { get { return GameObject.FindObjectOfType<SceneBase>(); } }

    public string GetSceneName(SceneType type)
    {
        string name = System.Enum.GetName(typeof(SceneType), type);
        return name;
    }

    public void LoadScene(SceneType type)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GetSceneName(type));
    }

    public void Dispose()
    {
        CurrentScene.Dispose();
    }
}