using UnityEngine;

public class CameraDynamicFeedback : MonoBehaviour
{
    CameraController cameraController;
    public GameObject feedbackObject;
    Vector3 initRot;

    void Start()
    {
        initRot.x = transform.eulerAngles.x;
        cameraController = UtilityClass.GetMainCamera().GetComponent<CameraController>();
    }

    void LateUpdate()
    {
        FreezeLocalRotation();

        if (cameraController.CameraIsLocked)
        {
            HideCameraDynamicFeedbackObject();
            return;
        }

        if (!feedbackObject.activeInHierarchy && UtilityClass.IsKeyMaintained(cameraController.CameraFocusOnTargetKey))
        {
            DisplayCameraDynamicFeedbackObject();
        }
        else if (feedbackObject.activeInHierarchy && UtilityClass.IsKeyUnpressed(cameraController.CameraFocusOnTargetKey))
        {
            HideCameraDynamicFeedbackObject();
        }
    }

    private void FreezeLocalRotation()
    {
        transform.eulerAngles = new Vector3(initRot.x, 0f, 0f);
    }

    void HideCameraDynamicFeedbackObject()
    {
        feedbackObject.SetActive(false);
    }

    void DisplayCameraDynamicFeedbackObject()
    {
        feedbackObject.SetActive(true);
    }
}