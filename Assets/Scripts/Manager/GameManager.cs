using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RodongSurviver.Manager
{
    public class GameManager : MonoBehaviour
    {
        public PlayerData PlayerData { get; set; }
        public EnforceData EnforceData { get; set; }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void PlayGame()
        {
            Time.timeScale = 1;
        }
    }
}