using System;
using UnityEngine;
using UniRx;

namespace RodongSurviver.Base
{    
    public abstract class ViewBase : MonoBehaviour
    {
        protected CompositeDisposable Disposables { get; set; } = new CompositeDisposable();
        public abstract void Show(Action onComplete);
        public abstract void ShowImmediate();
        public abstract void Hide(Action onComplete);
        public abstract void HideImmediate();

        public abstract void Dispose();

        protected float FadeInOutTimeSeconds { get; private set; } = 1.5f;
    }

}
