using UnityEngine;

public class VisibilityState : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer MyRenderer;
    private EntityDetection EntityDetection => GetComponent<EntityDetection>();

    [SerializeField] private bool isVisible;
    public bool IsVisible { get => isVisible; private set => isVisible = value; }

    private void Start() => InitVisibility();

    private void InitVisibility()
    {
        if (IsVisible) SetToVisible();
        if (!IsVisible) SetToInvisible();
    }

    public void SetToVisible()
    {
        MyRenderer.enabled = true;
        IsVisible = true;

        if (EntityDetection != null)
            EntityDetection.enabled = true;
    }

    public void SetToInvisible()
    {
        MyRenderer.enabled = false;
        IsVisible = false;

        if (EntityDetection != null)
            EntityDetection.enabled = false;
    }
}