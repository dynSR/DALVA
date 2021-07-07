using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    Vector3 initRot;

    void Start()
    {
        initRot = Vector3.zero;
    }

    void LateUpdate()
    {
        FreezeLocalRotation();
    }

    private void FreezeLocalRotation ()
    {
        Vector3 v3ToAssign = new Vector3(initRot.x, 0f, 0f);

        if (transform.eulerAngles != v3ToAssign)
        {
            transform.eulerAngles = v3ToAssign;
        }
    }
}
