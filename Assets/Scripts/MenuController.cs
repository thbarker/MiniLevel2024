using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        GoToStoryScreen();
    }

    public void GoToDirectionsScreen()
    {
        SceneManager.LoadScene("Directions");
    }

    public void GoToStoryScreen()
    {
        SceneManager.LoadScene("Story");
    }

    public void GoToMainLevel()
    {
        SceneManager.LoadScene("TheGame");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PlayAgain()
    {
        StartGame();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        #if UNITY_EDITOR
            // if in unity, stop playing mode
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // for standalone builds, quit the application
            Application.Quit();
        #endif
    }
}