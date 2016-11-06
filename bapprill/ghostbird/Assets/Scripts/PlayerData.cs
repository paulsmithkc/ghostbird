using UnityEngine;
using System.Collections;

public class PlayerData : ScriptableObject
{
    public int playerFood;
    public int babyFood;
    public int sleepLevel;

    private int playerFoodPause;
    private int babyFoodPause;
    private int sleepLevelPause;

    public float cutSceneBuffer = 20f;

    public float dataTimer = 0f;
    
    public void SetPauseValues()
    {
        playerFoodPause = playerFood;
        babyFoodPause = babyFood;
        sleepLevelPause = sleepLevel;
    }

    private void HoldPauseValues()
    {
        playerFood = playerFoodPause;
        babyFoodPause = babyFood;
        sleepLevelPause = sleepLevel;
    }

    public void StallResourceProgression(float deltaTime)
    {
        dataTimer += deltaTime;

        if(dataTimer < cutSceneBuffer)
        {

        }
    }
}
