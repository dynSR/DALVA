using UnityEngine;

public enum TypeOfEntity 
{ 
    None, 
    Self, 
    EnemyPlayer, Monster, EnemyMinion, EnemyStele,
    AllyPlayer, AllyMinion, AllyStele,
    Stele,
    Harvester 
}

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(VisibilityState))]
public class EntityDetection : MonoBehaviour
{
    [SerializeField] private TypeOfEntity typeOfEntity;
    public TypeOfEntity TypeOfEntity { get => typeOfEntity; set => typeOfEntity = value; }

    public Outline Outline => GetComponent<Outline>();

    private void Start() => SetOutlineColor();

    void SetOutlineColor()
    {
        switch (TypeOfEntity)
        {
            //case TypeOfEntity.None:
            //    break;
            //case TypeOfEntity.Self:
            //    Outline.OutlineColor = Color.grey;
            //    break;
            //case TypeOfEntity.EnemyPlayer:
            //    Outline.OutlineColor = Color.red;
            //    break;
            //case TypeOfEntity.Monster:
            //    Outline.OutlineColor = Color.red;
            //    break;
            //case TypeOfEntity.EnemyMinion:
            //    Outline.OutlineColor = Color.red;
            //    break;
            //case TypeOfEntity.EnemyStele:
            //    Outline.OutlineColor = Color.red;
            //    break;
            //case TypeOfEntity.AllyPlayer:
            //    Outline.OutlineColor = Color.blue;
            //    break;
            //case TypeOfEntity.AllyMinion:
            //    Outline.OutlineColor = Color.blue;
            //    break;
            //case TypeOfEntity.AllyStele:
            //    Outline.OutlineColor = Color.blue;
            //    break;
            case TypeOfEntity.Stele:
                Outline.OutlineColor = Color.yellow;
                break;
            case TypeOfEntity.Harvester:
                Outline.OutlineColor = Color.yellow;
                break;
        }

        Outline.enabled = false;
    }

    public void ActivateTargetOutlineOnHover(Outline targetOutlineFound, Color outlineColor)
    {
        targetOutlineFound.enabled = true;
        targetOutlineFound.OutlineColor = outlineColor;
    }

    public void DeactivateTargetOutlineOnHover(Outline targetOutlineFound)
    {
        targetOutlineFound.enabled = false;
    }
}