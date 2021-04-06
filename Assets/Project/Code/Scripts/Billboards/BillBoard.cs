using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviourPun
{
    [SerializeField] protected TextMeshProUGUI nameText;

    #region Ref
    public EntityDetection EntityDetection => GetComponentInParent<EntityDetection>();
    #endregion

    private Canvas Canvas => GetComponent<Canvas>();

    Vector3 initRot;

    protected virtual void Awake()
    {
        initRot.x = transform.eulerAngles.x;
        SetCharacterName();
    }

    protected virtual void Start()
    {
        Canvas.worldCamera = UtilityClass.GetMainCamera();
    }

    protected virtual void LateUpdate()
    {
        FreezeLocalRotation();
    }

    private void FreezeLocalRotation()
    {
        transform.eulerAngles = new Vector3(initRot.x, 0f, 0f);
    }

    void SetCharacterName()
    {
        //if (NameText != null)
        //NameText.text = GetPhotonNetworkUsername(); --> à ajouter dans la class Utility
    }
}