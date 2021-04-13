using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/// Un script pour une caméra libre.
/// 
/// Touches:
///	wasd / Flèches directionnelles	- mouvement caméra
///	q/e 			                - haut/bas (local space)
///	r/f 			                - haut/bas (world space)
///	pageup/pagedown	                - haut/bas (world space)
///	maintenir shift	                - Activivé le mode mouvement rapide
///	Souris droit  	                - Activé la caméra libre
///	Souris			                - caméra libre / rotation
///     

public class FreeCam : MonoBehaviour
{

    /// Vitesse normale du mouvement de la caméra.

    public float movementSpeed = 10f;


    /// Vitesse de mouvement de la caméra lorsque Shift est maintenu enfoncé.

    public float fastMovementSpeed = 100f;


    /// Sensibilité de la vue libre.

    public float freeLookSensitivity = 3f;


    /// Sensibilité à zoomer sur la caméra lors de l'utilisation de la molette de la souris.

    public float zoomSensitivity = 10f;


    /// Sensibilité à zoomer sur la caméra lors de l'utilisation de la molette de la souris (mode rapide).

    public float fastZoomSensitivity = 50f;


    /// Défini sur true lorsque vous etes en Free Look (sur le bouton droit de la souris).

    private bool looking = false;

    void Update()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
        {
            transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
        {
            transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
        }

        if (looking)
        {
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
            transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    void OnDisable()
    {
        StopLooking();
    }


    /// Activer la vue libre.

    public void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    /// Désactiver la vue libre.

    public void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
