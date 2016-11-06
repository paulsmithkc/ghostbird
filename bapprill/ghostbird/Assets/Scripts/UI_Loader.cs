using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_Loader : MonoBehaviour {

	public void LoadScene(int sceneValue)
    {
        SceneManager.LoadScene(sceneValue);
        Time.timeScale = 1f;
    }
}
