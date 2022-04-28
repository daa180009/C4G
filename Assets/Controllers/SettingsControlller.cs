using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsControlller : MonoBehaviour
{
    public void OnExit()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
