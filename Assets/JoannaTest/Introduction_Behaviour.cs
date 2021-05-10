using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Introduction_Behaviour : MonoBehaviour
{

    public GameObject blackOutSquare;


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
            StartCoroutine(FadeBlackOutSquare());
            Debug.Log("A key or mouse click has been detected");

        }

    }

   public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, int fadeSpeed =5)
    {
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmout;

        if (fadeToBlack)
        {
            while (blackOutSquare.GetComponent<Image>().color.a <1)
            {
                fadeAmout = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmout);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
                SceneManager.LoadScene("Scene_MainMenu");
            }
        }

        else
        {
            while (blackOutSquare.GetComponent<Image>().color.a> 0)
            {
                fadeAmout = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmout);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;

            }
        }
    }
}
