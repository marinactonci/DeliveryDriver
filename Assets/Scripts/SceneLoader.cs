using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadStartScene() {
        SceneManager.LoadScene("Start");
    }

    public void LoadHowToPlayScene()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void LoadMainScene() {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
