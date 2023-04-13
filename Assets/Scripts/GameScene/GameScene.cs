using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RodongSurviver.Base;

public class GameScene : SceneBase
{
    protected override void Initialize()
    {
        base.Initialize();

        SceneType = SceneType.Game;

    }

    public override void Dispose()
    {

    }
}
