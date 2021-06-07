using UnityEngine;

public class SteleOutline : MonoBehaviour
{
    public Outline[] outlines;
    public Color defaultColor;

    public void SetOutlineDefaultColor()
    {
        foreach (Outline outline in outlines)
        {
            if (outline.gameObject.activeInHierarchy)
            {
                outline.OutlineColor = defaultColor;
            }
        }
    }

    public void SetOutlineInteractionColor()
    {
        foreach (Outline outline in outlines)
        {
            if (outline.gameObject.activeInHierarchy)
            {
                outline.OutlineColor = Color.yellow;
            }
        }
    }
}
