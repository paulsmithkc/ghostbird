using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GoalCanvasDriver : MonoBehaviour {

    public Canvas canvas;

    public Image sky;
    public Image sun;
    public Image cloud;

    public Sprite smallSun;
    public Sprite medSun;
    public Sprite bigSun;

    public Sprite partCloud;
    public Sprite fullCloud;

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
        if (canvas.enabled != shouldShow)
        {
            //SetDistance(stage)
            //SetTemperature(stage)
        }
        canvas.enabled = shouldShow;
    }

    void SetDistance(int stage)
    {
        switch (stage)
        {
            case 1:
                sun.sprite = smallSun;
                break;
            case 2:
                sun.sprite = medSun;
                break;
            case 3:
                sun.sprite = bigSun;
                break;
            default:
                sun.sprite = smallSun;
                break;
        }
    }

    void SetTemperature(int stage)
    {
        Color tint = Color.white;
        Color sunColor = sun.color;
        switch (stage)
        {
            case 1:
                cloud.enabled = false;
                sunColor.a = 1f;
                break;
            case 2:
                cloud.enabled = false;
                sunColor.a = .8f;
                break;
            case 3:
                cloud.enabled = false;
                sunColor.a = .5f;
                break;
            case 4:
                cloud.enabled = true;
                sunColor.a = .3f;
                cloud.sprite = partCloud;
                break;
            case 5:
                cloud.enabled = true;
                sunColor.a = .15f;
                cloud.sprite = fullCloud;
                break;
            default:
                break;
        }

        sunColor.r = tint.r;
        sunColor.g = tint.g;
        sunColor.b = tint.b;

        sky.color = tint;
        sun.color = sunColor;
        cloud.color = tint;
    }
}
