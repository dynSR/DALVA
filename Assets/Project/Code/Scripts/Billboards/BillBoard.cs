using Photon.Pun;
using UnityEngine;

public class BillBoard : MonoBehaviourPun
{
    private Canvas Canvas => GetComponent<Canvas>();

    Vector3 initRot;

    protected virtual void Start()
    {
        Canvas.worldCamera = UtilityClass.GetMainCamera();
        initRot = transform.eulerAngles;
    }

    protected virtual void LateUpdate()
    {
        FreezeLocalRotation();
    }

    private void FreezeLocalRotation()
    {
        transform.eulerAngles = initRot;
    }
}