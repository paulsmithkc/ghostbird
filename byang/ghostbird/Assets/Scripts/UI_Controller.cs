using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Controller : MonoBehaviour {
    
    public GameObject slidersPanel;

    public GameObject pausePanel;

    public GameObject successPanel;

    public GameObject failurePanel;
    
    public Slider playerFood;
    public Slider babyFood;
    public Slider playerSleep;
    
    private Player player;
    private Baby baby;

    // Use this for initialization
    void Start() {

        player = GameObject.FindObjectOfType<Player>();
        baby = GameObject.FindObjectOfType<Baby>();

        playerFood.maxValue = Player._foodMax;
        babyFood.maxValue = Baby._foodMax;
        playerSleep.maxValue = Player._tiredMax;

        slidersPanel.SetActive(true);
    }
	
	// Update is called once per frame
	void Update() {

        playerFood.value = player._foodCurrent;
        playerSleep.value = player._tiredCurrent;
        babyFood.value = baby._foodCurrent;

        if (player._state == Player.PlayerState.DEAD ||
            baby._state == Baby.BabyState.DEAD)
        {
            FailureState();
        }
	}

    public void SuccessState()
    {
        Time.timeScale = 0f;
        successPanel.SetActive(true);
        slidersPanel.SetActive(false);
    }

    public void FailureState()
    {
        Time.timeScale = 0f;
        failurePanel.SetActive(true);
        slidersPanel.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        slidersPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        slidersPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
