using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviourPun
{
    [SerializeField] protected TextMeshProUGUI nameText;

    #region Ref
    protected EntityStats Stats => GetComponentInParent<EntityStats>();
    #endregion

    private Canvas Canvas => GetComponent<Canvas>();

    Vector3 initRot;

    protected virtual void Awake()
    {
        Canvas.worldCamera = UtilityClass.GetMainCamera();
        initRot.x = transform.eulerAngles.x;

        SetCharacterName();
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