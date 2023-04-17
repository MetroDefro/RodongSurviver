using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCanvasView : MonoBehaviour
{
    public Button PlayButton => playButton;
    public Button RetryButton => retryButton;

    [SerializeField] private Button playButton;
    [SerializeField] private Button retryButton;
}
