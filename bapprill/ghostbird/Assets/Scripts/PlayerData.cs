using UnityEngine;
using System.Collections;

public class PlayerData : ScriptableObject {

    private int playerFood;
    private int babyFood;
    private int sleepFood;

    public int PlayerFood
    {
        get
        {
            return playerFood;
        }

        set
        {
            playerFood = value;
        }
    }

    public int BabyFood
    {
        get
        {
            return babyFood;
        }

        set
        {
            babyFood = value;
        }
    }

    public int SleepFood
    {
        get
        {
            return sleepFood;
        }

        set
        {
            sleepFood = value;
        }
    }
}
