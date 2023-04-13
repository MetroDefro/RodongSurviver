using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RodongSurviver.Base
{
    public enum SceneType
    {
        Unknown = 0,
        Main = 1,
        Game = 2,
    }

    public abstract class SceneBase : MonoBehaviour
    {
        public SceneType SceneType { get; protected set; } = SceneType.Unknown;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public abstract void Dispose();
    }

}