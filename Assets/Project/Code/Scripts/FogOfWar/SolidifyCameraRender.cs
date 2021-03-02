using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SolidifyCameraRender : MonoBehaviour
{
    public Shader flatShader;
    Camera cam;
	void OnEnable ()
	{
	    cam = GetComponent<Camera>();
        cam.SetReplacementShader(flatShader, "");
	}
	
}
