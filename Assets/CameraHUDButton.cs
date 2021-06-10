using UnityEngine;

public class CameraHUDButton : MonoBehaviour
{
    CameraController cameraController;
    public GameObject padlockObject;

    void Start()
    {
        cameraController = UtilityClass.GetMainCamera().GetComponent<CameraController>();
    }

    void LateUpdate()
    {
        if (cameraController.CameraIsLocked && !padlockObject.activeInHierarchy)
        {
            DisplayPadlock();
        }
        else if (cameraController.CameraIsUnlocked && padlockObject.activeInHierarchy)
        {
            HidePadlock();
        }

        if (UtilityClass.IsKeyMaintained(cameraController.CameraFocusOnTargetKey) && !padlockObject.activeInHierarchy)
        {
            DisplayPadlock();
        }
        else if (UtilityClass.IsKeyUnpressed(cameraController.CameraFocusOnTargetKey) && padlockObject.activeInHierarchy && cameraController.CameraIsUnlocked)
        {
            HidePadlock();
        }
    }

    private void DisplayPadlock()
    {
        padlockObject.SetActive(true);
    }

    private void HidePadlock()
    {
        padlockObject.SetActive(false);
    }
}
