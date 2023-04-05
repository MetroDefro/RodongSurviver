using UnityEngine;
using System;
using UniRx;

namespace RodongSurviver.Base
{
    public abstract class PresenterBase : MonoBehaviour
    {
        #region [ Properties ]
        protected CompositeDisposable Disposables { get; set; } = new CompositeDisposable();
        #endregion

        #region [ Variables ]
        protected TimeSpan ClickThrottleFirstTime = TimeSpan.FromSeconds(0.3f);
        #endregion

        #region [ MonoBehaviour Messages ]

        protected void OnDestroy()
        {
            if (Disposables != null)
                Disposables.Clear();

            Dispose();
        }

        protected void Update()
        {

        }
        #endregion

        #region [ Abstract methods ]
        public virtual void Dispose()
        {
            if (Disposables != null)
                Disposables.Clear();
        }
        #endregion

        #region [ Private methods ]
        #endregion
    }

}
