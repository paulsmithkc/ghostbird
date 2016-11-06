using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroAct1 : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        cameraTransform = Camera.main.transform;
        cameraTransform.position = eyeShotCameraSnapTransform.position;
        timeElapsed = 0f;
        introspectionContainer.enabled = true;
        introspectionContainer.gameObject.SetActive(true);
        introspectionText.enabled = true;
        introspectionText.gameObject.SetActive(true);
        isDismissed = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeElapsed += Time.deltaTime;

        DoIntrospection();
        DoFadeInFromWhite();
        DoZoomOutToShowBigBird();
        DoZoomOutToShowBirdsFlyingAndLevelContext();
        DoGiveGoalScreenHint();

        DoFollowPlayerWithCamera();
    }

    void DoIntrospection()
    {
        string introspectionStr = "";

        if (timeElapsed >= 0f && timeElapsed < 3f)
        {
            introspectionStr = "My love came late. My child came late.";
        }
        else if (timeElapsed >= 3f && timeElapsed < 6f)
        {
            introspectionStr = "Do I regret? I do not know. There is no time to think.";
        }
        else if (timeElapsed >= 6f && timeElapsed < 9f)
        {
            introspectionStr = "I can feel it in the air.";
        }
        else if (timeElapsed >= 9f && timeElapsed < 12f)
        {
            introspectionStr = "Winter approaches.";
        }

        if (introspectionStr.Length > 0)
        {
            introspectionContainer.enabled = introspectionStr.Length > 0;
            introspectionContainer.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.enabled = introspectionStr.Length > 0;
            introspectionText.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.text = introspectionStr;
        } else if (!isDismissed && timeElapsed >= 12f)
        {
            isDismissed = true;
            introspectionContainer.enabled = introspectionStr.Length > 0;
            introspectionContainer.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.enabled = introspectionStr.Length > 0;
            introspectionText.gameObject.SetActive(introspectionStr.Length > 0);
            introspectionText.text = introspectionStr;
        }
    }

    void DoFadeInFromWhite()
    {
        if (timeElapsed >= 12f && timeElapsed < 13f)
        {
            fader.FadeTo(new Color(255, 255, 255, 0), 1f);
        }
    }

    void DoZoomOutToShowBigBird()
    {
        if (timeElapsed >= 13f && timeElapsed < 16f)
        {
            MoveCamera(eyeShotCameraSnapTransform, extremeCloseCameraSnapTransform, 16f);
        }
    }

    void DoZoomOutToShowBirdsFlyingAndLevelContext()
    {
        if (timeElapsed >= 17f && timeElapsed < 18f)
        {
            MoveCamera(extremeCloseCameraSnapTransform, fullLevelCameraSnapTransform, 18f);
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
        if (timeElapsed >= 18f)
        {
            cameraTransform.position = fullLevelCameraSnapTransform.position;
        }
    }


}
