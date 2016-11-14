using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_Loader : MonoBehaviour {

	public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
