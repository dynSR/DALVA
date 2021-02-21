using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Player_MinionsPos : MonoBehaviour
{
    [SerializeField]
    private List<Transform> baseListOfMinionsPos = new List<Transform>();
    public List<Transform> MinionPosAvaliable = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            Debug.Log(transform.GetChild(i).name);
            baseListOfMinionsPos.Add(transform.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P is pressed");
            CheckPosAvaliable();
        }
    }

    private void CheckPosAvaliable()
    {
        MinionPosAvaliable.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (baseListOfMinionsPos[i].childCount < 1)
            {
                //Debug.Log(baseListOfMinionsPos[i].name);
                MinionPosAvaliable.Add(baseListOfMinionsPos[i]);
            }
        }
    }

    public Transform AssignMinionPos()
    {
        CheckPosAvaliable();
        return MinionPosAvaliable[0];
    }
}
