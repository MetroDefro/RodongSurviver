using RodongSurviver.Components.DiedPanel;
using UnityEngine;

namespace RodongSurviver.State
{
    public class MainSceneState : MonoBehaviour
    {
        #region [ Variables ]
        [SerializeField] private DiedPanelPresenter diedPanelPresenter;
        [SerializeField] private GameManager gameManager;
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
            diedPanelPresenter.Initialize(new DiedPanelPresenter.DiedPanelActions()
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
