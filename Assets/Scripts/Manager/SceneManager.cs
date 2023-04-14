using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RodongSurviver.Base;

namespace RodongSurviver.Manager
{
    public enum SceneType
    {
        Unknown = 0,
        Main = 1,
        Game = 2,
    }

    public class SceneManager: MonoBehaviour
    {
        public SceneType CurrentSceneType { get; private set; } = SceneType.Unknown;

        public string GetSceneName(SceneType type)
        {
            string name = System.Enum.GetName(typeof(SceneType), type);
            return name;
        }

        public void LoadSceneAsync(SceneType type)
        {
            StartCoroutine(LoadingScene());

            IEnumerator LoadingScene()
            {
                AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GetSceneName(type), LoadSceneMode.Single);
                operation.allowSceneActivation = false;

                while (!operation.isDone)
                {
                    yield return null;

                    if(operation.progress >= 0.9f)
                    {
                        operation.allowSceneActivation = true;
                    }
                }
            }
        }
    }
}