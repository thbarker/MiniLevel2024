using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    public void PlayGameAgain()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
