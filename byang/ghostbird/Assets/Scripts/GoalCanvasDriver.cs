using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalCanvasDriver : MonoBehaviour {

    public Image sun;
    public Image cloud;
    private bool isActive;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!isActive)
        {
            if (Input.GetMouseButton(1))
            {
                ShowGoalCanvas(true);
            }
        }
	}

    void ShowGoalCanvas(bool shouldShow)
    {
        if (shouldShow)
        {

        }
    }
}
