using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Controller : MonoBehaviour {
    
    public GameObject pausePanel;

    public GameObject successPanel;

    public GameObject failurePanel;

    public PlayerData playerData;

    public Slider playerFood;
    public Slider babyFood;
    public Slider playerSleep;

    private int playerFoodCurrent;
    private int playerSleepCurrent;

    private Player player;
    
    // Use this for initialization
    void Start () {

        player = FindObjectOfType<Player>().GetComponent<Player>();

        playerFoodCurrent = (int)playerFood.GetComponent<Slider>().value;
        playerSleepCurrent = (int)playerSleep.GetComponent<Slider>().value;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        
        if (player._foodCurrent != playerFoodCurrent)
        {
            if (player._foodCurrent == 0)
            {
                playerFood.value = player._foodCurrent;
                FailureState();
                return;
            }

            else {

                playerFood.value = player._foodCurrent;
                playerFoodCurrent = player._foodCurrent;
            }
        }
        
        if(player._tiredCurrent != playerSleepCurrent)
        {
            playerSleep.value = (int)player._tiredCurrent;
            playerSleepCurrent = (int)player._tiredCurrent;
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
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
