using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfEntity { None, Self, Enemy, Ally, Stele, Harvester }

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(VisibilityState))]
public class EntityDetection : MonoBehaviour
{
    [SerializeField] private TypeOfEntity typeOfEntity;
    public TypeOfEntity TypeOfEntity { get => typeOfEntity; }

    private Outline Outline => GetComponent<Outline>();

    private void Start() => SetOutlineColor();

    void SetOutlineColor()
    {
        switch (TypeOfEntity)
        {
            case TypeOfEntity.None:
                enabled = false;
                break;
            case TypeOfEntity.Self:
                Outline.OutlineColor = Color.white;
                break;
            case TypeOfEntity.Enemy:
                Outline.OutlineColor = Color.red;
                break;
            case TypeOfEntity.Ally:
                Outline.OutlineColor = Color.blue;
                break;
            case TypeOfEntity.Stele:
                Outline.OutlineColor = Color.yellow;
                break;
            case TypeOfEntity.Harvester:
                Outline.OutlineColor = Color.yellow;
                break;
            default:
                break;
        }

        Outline.enabled = false;
    }
}