using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroAct2 : MonoBehaviour {

    public Transform eyeShotCameraSnapTransform;
    public Transform extremeCloseCameraSnapTransform;
    public Transform fullLevelCameraSnapTransform;
    public Transform playerTransform;
    public Image introspectionContainer;
    public Text introspectionText;
    public HudFade fader;
    private float timeElapsed;
    private bool isDismissed;

    // Use this for initialization
    void Start ()
    {
        timeElapsed = 0f;
        isDismissed = false;

    }
	
	// Update is called once per frame
	void Update () {

        timeElapsed += Time.deltaTime;

        DoIntrospection();
    }

    void DoIntrospection()
    {
        string introspectionStr = "";

        if (timeElapsed >= 0f && timeElapsed < 3f)
        {
            introspectionStr = "A nest? Big birds don't need nests.";
        }
        else if (timeElapsed >= 3f && timeElapsed < 6f)
        {
            introspectionStr = "Be strong child. I'm here for you.";
        }

        if (introspectionStr.Length > 0)
        {
            introspectionContainer.enabled = introspectionStr.Length > 0;
            introspectionContainer.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.enabled = introspectionStr.Length > 0;
            introspectionText.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.text = introspectionStr;
        }
        else if (!isDismissed && timeElapsed >= 6f)
        {
            isDismissed = true;
            introspectionContainer.enabled = introspectionStr.Length > 0;
            introspectionContainer.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.enabled = introspectionStr.Length > 0;
            introspectionText.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.text = introspectionStr;
        }
    }
}
