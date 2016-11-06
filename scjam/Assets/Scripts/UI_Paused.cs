using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Paused : MonoBehaviour {

    private bool gamePaused;

    public GameObject pausePanel;

    public GameObject successPanel;

    public GameObject failurePanel;

    // Use this for initialization
    void Start () {

        gamePaused = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }   
	}

    public void SuccessState()
    {
        Time.timeScale = 0f;
        successPanel.SetActive(true);
    }

    public void FailureState()
    {
        Time.timeScale = 0f;
        failurePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
