using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Event1OutTrigger : MonoBehaviour {

    public Image introspectionContainer;
    public Text introspectionText;
    private bool isTriggered;
    private bool isDismissed;
    private float triggerTime;

    private const float TEXT_UPTIME = 3f;

	// Use this for initialization
	void Start () {
        isTriggered = false;
        isDismissed = false;
        triggerTime = 0f;
    }

    void Update()
    {
        if (isTriggered && !isDismissed)
        {
            Debug.Log("Update is triggered. triggerTime=" + triggerTime + "; curtime=" + Time.time);
            string str = "";
            if (Time.time < triggerTime + TEXT_UPTIME)
            {
                str = "Child of mine... it is already autumn. The sun is retreating.";
            } else if (Time.time < triggerTime + TEXT_UPTIME * 2)
            {
                str = "You cannot fly, but we must head South.";
            }

            Debug.Log("str=" + str);
            if (str.Length == 0 && !isDismissed)
            {
                isDismissed = true;
                introspectionContainer.enabled = false;
                introspectionContainer.gameObject.SetActive(false);
                introspectionText.enabled = false;
                introspectionText.gameObject.SetActive(false);
                introspectionText.text = str;
                Debug.Log("Dismissed!");
            } else if (str.Length > 0)
            {
                introspectionContainer.enabled = true;
                introspectionContainer.gameObject.SetActive(true);
                introspectionText.enabled = true;
                introspectionText.gameObject.SetActive(true);
                introspectionText.text = str;
                Debug.Log("The text has been set sir!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered an enter!");
        if (!isTriggered && string.Equals(Player.PLAYER_TAG, other.tag))
        {
            Debug.Log("It's a player!");
            isTriggered = true;
            triggerTime = Time.time;
        }
    }
}
