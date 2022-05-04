using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class MainMenuController : MonoBehaviour
{
    public GameObject loadGameButton;
    public WorldInfo worldInfo;

    string GetFileDirectory()
    {
        return Application.persistentDataPath + "/SavedGames";
    }

    string GetFilename()
    {
        return GetFileDirectory() + "/" + "mainSave" + ".json";
    }

    public void Start()
    {
        loadGameButton.SetActive(File.Exists(GetFilename()));
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("DeckBuilding");
    }

    public void LoadGame()
    {
        if (File.Exists(GetFilename()))
        {
            worldInfo.Restart();

            string jsonFile = File.ReadAllText(GetFilename());
            LevelSaveData data = JsonUtility.FromJson<LevelSaveData>(jsonFile);
            PlayerChoices.SelectedStage = data.savedStage;

            SceneManager.LoadSceneAsync("Level");
        }
    }

    public void Settings()
    {
        SceneManager.LoadSceneAsync("Settings");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
