using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using RodongSurviver.Base;
using RodongSurviver.Manager;

public class MainScenePresenter : PresenterBase
{
    [SerializeField] private PlayerSelectPresenter playerSelectPresenter;

    private MainSceneView view;

    private GameManager gameManager;
    private SceneManager sceneManager;

    private void Start()
    {
        Initialize();
    }

    [Inject]
    public void Inject(GameManager gameManager, SceneManager sceneManager)
    {
        this.gameManager = gameManager;
        this.sceneManager = sceneManager;
    }

    public override void Dispose()
    {
        base.Dispose();
        view.Dispose();
    }

    public void Initialize()
    {
        if (TryGetComponent(out MainSceneView view))
        {
            this.view = view;
        }

        playerSelectPresenter.Initialize(
            () => sceneManager.LoadSceneAsync(SceneType.Game), 
            (playerData) => SetPlayerData(playerData));
    }

    private void SetPlayerData(PlayerData playerData)
    {
        gameManager.playerData = playerData;
    }
}
