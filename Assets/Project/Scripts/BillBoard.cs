using Photon.Pun;
using TMPro;
using UnityEngine;

public class BillBoard : MonoBehaviourPun
{
    private Canvas Canvas => GetComponent<Canvas>();

    [Header("BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerHealthBar;
    [SerializeField] private TextMeshProUGUI playerLevelText;

    private void Awake()
    {
        Canvas.worldCamera = UtilityClass.GetMainCamera();
        //playerNameText.text = GetPhotonNetworkUsername(); --> à ajouter dans la class Utility
    }

    void LateUpdate()
    {
        //if (UtilityClass.GetMainCamera().GetComponent<CameraController>().CameraIsLocked)
            LookAtMainCamera();
    }

    private void LookAtMainCamera()
    {
        transform.LookAt(UtilityClass.GetMainCameraPosition());
        transform.Rotate(0, 180, 0);
    }
}