using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("DeckBuilding");
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
