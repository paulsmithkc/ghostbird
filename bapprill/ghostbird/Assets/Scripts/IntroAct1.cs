using UnityEngine;
using System.Collections;

public class IntroAct1 : MonoBehaviour {

    public Transform eyeShotCameraSnapTransform;
    public Transform extremeCloseCameraSnapTransform;
    public Transform fullLevelCameraSnapTransform;
    public Transform playerTransform;
    private Transform cameraTransform;
    private float timeElapsed;

	// Use this for initialization
	void Start () {
        cameraTransform = Camera.main.transform;
        cameraTransform.position = eyeShotCameraSnapTransform.position;
        timeElapsed = 0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed <= 1f)
        {

        } else if (timeElapsed < 3f)
        {
            MoveCamera(eyeShotCameraSnapTransform, extremeCloseCameraSnapTransform, 3f);
        } else if (timeElapsed > 4f && timeElapsed < 5f)
        {
            MoveCamera(extremeCloseCameraSnapTransform, fullLevelCameraSnapTransform, 5f);
        }
    }

    void MoveCamera(Transform from, Transform to, float endTime)
    {
        cameraTransform.position = Vector3.Lerp(to.position, from.position, endTime - timeElapsed);
    }
    
}
