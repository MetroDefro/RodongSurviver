using RodongSurviver.Components.DiedPanel;
using UnityEngine;

namespace RodongSurviver.State
{
    public class GameSceneState : MonoBehaviour
    {
        #region [ Variables ]
        [SerializeField] private ResultPanelPresenter diedPanelPresenter;
        [SerializeField] private GameScenePresenter gameManager;
        #endregion

        #region [ MonoBehaviour Messages ]
        private void Awake()
        {
            InitializeGameManager();

            InitializeDiedPanel();
        }
        #endregion

        #region [ Private Methods ]
        #region [ Initialize Panels ]
        private void InitializeGameManager()
        {
            gameManager.SetupDiedAction(() =>
            {
                diedPanelPresenter.gameObject.SetActive(true);
            });
        }

        private void InitializeDiedPanel()
        {
            diedPanelPresenter.Initialize(new ResultPanelPresenter.DiedPanelActions()
            {
                RetryEvent = () => 
                {
                    diedPanelPresenter.gameObject.SetActive(false);
                    gameManager.OnReset();
                }
            });
        }
        #endregion
        #endregion
    }
}
