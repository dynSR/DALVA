using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuRedirector : MonoBehaviour
{
    private MainMenuUIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = GetComponentInChildren<MainMenuUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameEvent()
    {
        uiManager.StartGame();
    }
}
