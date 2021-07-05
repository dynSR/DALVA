using UnityEngine;

public class GameParameters : MonoBehaviour
{
    public bool classIsMage = false;

    public int maxLevelDone;

    #region Singleton
    public static GameParameters Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
    #endregion

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            if(maxLevelDone  < 3)
            {
                maxLevelDone++;
            }
        }
    }

    public void SetClassChosenToMage()
    {
        classIsMage = true;
    }

    public void SetClassChosenToWarrior()
    {
        classIsMage = false;
    }
}
