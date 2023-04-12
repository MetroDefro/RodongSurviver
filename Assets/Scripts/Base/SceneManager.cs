using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager
{
    private List<SceneBase> activeScenes = new List<SceneBase>();
    public SceneBase CurrentScene { get { return GameObject.FindObjectOfType<SceneBase>(); } }

    public string GetSceneName(SceneType type)
    {
        string name = System.Enum.GetName(typeof(SceneType), type);
        return name;
    }

    public void SetScene()
    {

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