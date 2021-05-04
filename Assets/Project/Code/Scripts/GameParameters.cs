using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameters : MonoBehaviour
{
    #region Fields
    //singleton
    public static GameParameters singleton;

    //Game informations
    public bool characterClass;
    public int lifeMalus;
    public int attackMalus;
    public int speedMalus;
    public int steleMalus;

    #endregion

    private void Awake()
    {
        if (singleton == null) singleton = this;
        else Destroy(this);
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        characterClass = false;
        lifeMalus = 0;
        attackMalus = 0;
        speedMalus = 0;
        steleMalus = 0;

    }

    public void SetGameParameters(bool charaClass, int lifeMalusValue, int attackMalusValue, int speedMalusValue, int steleMalusValue)
    {
        characterClass = charaClass;
        lifeMalus = lifeMalusValue;
        attackMalus = attackMalusValue;
        speedMalus = speedMalusValue;
        steleMalus = steleMalusValue;
    }
}
