using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Introduction_Behaviour : MonoBehaviour
{




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Detects if any key has been pressed.
        if (Input.anyKey)
        {

            SceneManager.LoadScene("Scene_MainMenu");
            Debug.Log("A key or mouse click has been detected");

        }

    }

 
}
