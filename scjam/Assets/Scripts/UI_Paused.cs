using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Paused : MonoBehaviour {

    private bool gamePaused;

	// Use this for initialization
	void Start () {

        gamePaused = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            
        }   
	}

    public void ResumeGame()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
