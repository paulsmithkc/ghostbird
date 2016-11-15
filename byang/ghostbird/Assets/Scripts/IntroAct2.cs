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
    private Transform cameraTransform;
    private float timeElapsed;
    private bool isDismissed;
    private bool isDone;

    // Use this for initialization
    void Start ()
    {
        cameraTransform = Camera.main.transform;
        cameraTransform.position = fullLevelCameraSnapTransform.position;
        timeElapsed = 0f;
        introspectionContainer.enabled = true;
        introspectionContainer.gameObject.SetActive(true);
        introspectionText.enabled = true;
        introspectionText.gameObject.SetActive(true);
        isDismissed = false;
        Time.timeScale = 0f;
        isDone = false;
    }
	
	// Update is called once per frame
	void Update () {
        timeElapsed += Time.unscaledDeltaTime;

        DoIntrospection();
        DoFadeInFromWhite();
        DoGiveGoalScreenHint();

        DoFollowPlayerWithCamera();
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
            introspectionStr = "Be strong child.";
        }
        else if (timeElapsed >= 6f && timeElapsed < 9f)
        {
            introspectionStr = "I'm here for you.";
        }

        bool showText = introspectionStr.Length > 0;
        if (showText)
        {
            introspectionContainer.enabled = showText;
            introspectionContainer.gameObject.SetActive(showText);
            introspectionText.enabled = showText;
            introspectionText.gameObject.SetActive(showText);
            introspectionText.text = introspectionStr;
        }
        else if (!isDismissed && timeElapsed >= 9f)
        {
            isDismissed = true;
            introspectionContainer.enabled = introspectionStr.Length > 0;
            introspectionContainer.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.enabled = introspectionStr.Length > 0;
            introspectionText.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.text = introspectionStr;
            Time.timeScale = 1f;
        }
    }

    void DoFadeInFromWhite()
    {
        if (!isDone && timeElapsed >= 9f && timeElapsed < 10f)
        {
            fader.FadeTo(new Color(255, 255, 255, 0), 1f);
            isDone = true;
        }
    }

    void MoveCamera(Transform from, Transform to, float endTime)
    {
        cameraTransform.position = Vector3.Lerp(to.position, from.position, endTime - timeElapsed);
    }

    void DoGiveGoalScreenHint()
    {

    }

    void DoFollowPlayerWithCamera()
    {
        if (timeElapsed >= 10f)
        {
            cameraTransform.position = fullLevelCameraSnapTransform.position;
        }
    }
}
