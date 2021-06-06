using UnityEngine;

public class MiniMapIconLogic : MonoBehaviour
{
    public EntityStats Stats;
    public bool IsABossEntity = false;

    private void OnEnable()
    {
        if (Stats == null)
        {
            Debug.LogError("Missing stats reference, please populate" + transform);
            return;
        }
        

        Stats.OnEntityDeath += HideMiniMapIcon;
        Stats.OnEntityRespawn += DisplayMiniMapIcon;

        if (IsABossEntity)
            FreezeLocalRotation();
    }

    private void OnDisable()
    {
        if (Stats == null)
        {
            Debug.LogError("Missing stats reference, please populate" + transform);
            return;
        }

        Stats.OnEntityDeath -= HideMiniMapIcon;
        Stats.OnEntityRespawn -= DisplayMiniMapIcon;
    }


    protected virtual void LateUpdate()
    {
        if (IsABossEntity)
            FreezeLocalRotation();
    }

    private void FreezeLocalRotation()
    {
        transform.eulerAngles = new Vector3(90, 0f, 0f);
    }

    void DisplayMiniMapIcon()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(true);
        }
        
    }

    void HideMiniMapIcon()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
    }
}