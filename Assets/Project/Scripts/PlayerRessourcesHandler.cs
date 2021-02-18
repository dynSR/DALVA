using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRessourcesHandler : MonoBehaviour
{
    [Header("PLAYER SHOP")]
    [SerializeField] private OpenCloseShopWindow playerShopWindow;
    [SerializeField] private Inventory playerInventory;

    [Header("PLAYER RESSOURCES")]
    [SerializeField] private float amountOfPlayerRessources = 500;

    public OpenCloseShopWindow PlayerShopWindow { get => playerShopWindow; set => playerShopWindow = value; }
    public Inventory PlayerInventory { get => playerInventory; set => playerInventory = value; }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddRessources(500);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            RemoveRessources(250);
        }
    }

    private void AddRessources(int amountToAdd)
    {
        amountOfPlayerRessources += amountToAdd;
    }

    private void RemoveRessources(int amountToRemove)
    {
        amountOfPlayerRessources -= amountToRemove;
    }
}
