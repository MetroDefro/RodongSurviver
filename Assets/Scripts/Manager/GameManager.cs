using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

namespace RodongSurviver.Manager
{
    public class GameManager : MonoBehaviour
    {
        public int CurrentLanguage { get; set; }

        public PlayerData PlayerData { get; set; }
        public EnforceData EnforceData { get; set; }


        private void Awake()
        {
            Application.targetFrameRate = 60;    
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void PlayGame()
        {
            Time.timeScale = 1;
        }

        public void PluseMoney(int value)
        {
            EnforceData.Money += value;
            SaveEnforceData(EnforceData);
        }

        public void MinusMoney(int value)
        {
            EnforceData.Money -= value;
            SaveEnforceData(EnforceData);
        }

        public void SaveEnforceData(EnforceData enforceData)
        {
            string path = Application.persistentDataPath + "/savedata.json";

            string json = JsonUtility.ToJson(enforceData);

            File.WriteAllText(path, json);
        }

        public EnforceData LoadEnforceData()
        {
            string path = Application.persistentDataPath + "/savedata.json";

            if (File.Exists(path))
            {
                EnforceData enforceData = new EnforceData();

                string loadJson = File.ReadAllText(path);
                enforceData = JsonUtility.FromJson<EnforceData>(loadJson);

                return enforceData;
            }
            else
            {
                return null;
            }

        }
    }
}