using UnityEngine;

public class VisibilityState : MonoBehaviour
{
    [Header("ATTRIBUTES")]
    [SerializeField] private Renderer myRenderer;
    [SerializeField] private GameObject miniMapIcon;
    [SerializeField] private GameObject billboard;
    [SerializeField] private int timeSeen = 0; //public to debug
    [SerializeField] private bool visibilityStateCanChange = true;

    #region Refs
    private CharacterStat Character => GetComponent<CharacterStat>();
    private EntityDetection EntityDetection => GetComponent<EntityDetection>();
    #endregion

    [SerializeField] private bool isVisible; //public to debug
    public bool IsVisible { get => isVisible; private set => isVisible = value; }

    private void Start() => InitVisibility();

    private void InitVisibility()
    {
        if (myRenderer == null) return;

        if (IsVisible) SetToVisible();
        if (!IsVisible) SetToInvisible();
    }

    public void SetToVisible()
    {
        if (!visibilityStateCanChange) return;

        if (FogOfWarManager.Instance.EntityIsContained(transform)) timeSeen++;

        if (!FogOfWarManager.Instance.EntityIsContained(transform))
        {
            FogOfWarManager.Instance.VisibleEntities.Add(transform);
            timeSeen++;

            ActivateRenderer(myRenderer);

            Debug.Log("Was invisible");
        }
    }

    public void SetToInvisible()
    {
        if (!visibilityStateCanChange) return;

        if (timeSeen > 0) timeSeen--;

        if (FogOfWarManager.Instance.EntityIsContained(transform)
            && timeSeen == 0)
        {
            FogOfWarManager.Instance.VisibleEntities.Remove(transform);
            DeactivateRenderer(myRenderer);

        }
        else if (!FogOfWarManager.Instance.EntityIsContained(transform)
            && timeSeen == 0)
        {
            DeactivateRenderer(myRenderer);
        }
    }

    void ActivateRenderer(Renderer renderer)
    {
        myRenderer.enabled = true;
        IsVisible = true;

        if (billboard != null)
            billboard.SetActive(true);

        if (miniMapIcon != null)
            miniMapIcon.SetActive(true);

        if (EntityDetection != null)
            EntityDetection.enabled = true;
    }

    void DeactivateRenderer(Renderer renderer)
    {
        myRenderer.enabled = false;
        IsVisible = false;

        if (billboard != null)
            billboard.SetActive(false);

        if (miniMapIcon != null)
            miniMapIcon.SetActive(false);

        if (EntityDetection != null)
            EntityDetection.enabled = false;
    }
}