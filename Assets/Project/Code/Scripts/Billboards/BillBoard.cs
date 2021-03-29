using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BillBoard : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;

    #region Ref
    protected CharacterStat Stats => GetComponentInParent<CharacterStat>();
    #endregion

    private Canvas Canvas => GetComponent<Canvas>();

    Vector3 initRot;

    private void OnEnable()
    {
        //Call event for when character is stunned 
    }

    private void OnDisable()
    {
        
    }

    protected virtual void Awake()
    {
        Canvas.worldCamera = UtilityClass.GetMainCamera();
        initRot = transform.eulerAngles;

        SetCharacterName();
    }

    protected virtual void LateUpdate()
    {
        FreezeLocalRotation();
    }

    private void FreezeLocalRotation()
    {
        transform.eulerAngles = initRot;
    }

    void SetCharacterName()
    {
        //if (NameText != null)
        //NameText.text = GetPhotonNetworkUsername(); --> à ajouter dans la class Utility
    }
}