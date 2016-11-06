using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_Loader : MonoBehaviour {

	public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
