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
            } else if (str.Length > 0)
            {
                introspectionContainer.enabled = true;
                introspectionContainer.gameObject.SetActive(true);
                introspectionText.enabled = true;
                introspectionText.gameObject.SetActive(true);
                introspectionText.text = str;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && string.Equals(Player.PLAYER_TAG, other.tag))
        {
            isTriggered = true;
            triggerTime = Time.time;
        }
    }
}
