using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviourPun
{
    [SerializeField] protected TextMeshProUGUI nameText;

    #region Ref
    protected EntityDetection EntityDetection => GetComponentInParent<EntityDetection>();
    private EntityStats stats;
    #endregion

    private Canvas Canvas => GetComponent<Canvas>();

    Vector3 initRot;

    protected virtual void Awake()
    {
        initRot.x = transform.eulerAngles.x;
        SetCharacterName();

        if (GetComponentInParent<EntityStats>() != null)
        {
            stats = GetComponentInParent<EntityStats>();
        }
    }

    protected virtual void OnEnable()
    {
        stats.OnEntityDeath += HideBillboard;
        stats.OnEntityRespawn += DisplayBillboard;
    }

    protected virtual void OnDisable()
    {
        stats.OnEntityDeath -= HideBillboard;
        stats.OnEntityRespawn -= DisplayBillboard;
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
        Vector3 v3ToAssign = new Vector3(initRot.x, 0f, 0f);

        if (transform.eulerAngles != v3ToAssign)
        {
            transform.eulerAngles = v3ToAssign;
        }
    }

    void SetCharacterName()
    {
        //if (NameText != null)
        //NameText.text = GetPhotonNetworkUsername(); --> à ajouter dans la class Utility
    }

    void HideBillboard()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void DisplayBillboard()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}