using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody; // gameobject for player body
    public Transform playerHead; // gameobject for player head
    public float sensMod = 2f; // modifier for sensitivity

    [HideInInspector]
    public SettingsMenu settings; // access player's settings

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // lock cursor in center of screen
        settings = gameObject.GetComponent<SettingsMenu>(); // get settings access
    }

    // Update is called once per frame
    void Update()
    {
        if (settings != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * settings.mouseSensX * sensMod * Time.deltaTime; // get mouse movement along x axis
            float mouseY = Input.GetAxis("Mouse Y") * settings.mouseSensY * sensMod * Time.deltaTime; // get mouse movement along y axis

            xRotation -= mouseY; // set new mouse rotation
            // for clamp, first value is up, second is down
            xRotation = Mathf.Clamp(xRotation, -90f, 60f); // restrict head movement between +- 80 degrees

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // apply this vertical mouse movement as a rotation to player head
            playerBody.Rotate(Vector3.up * mouseX); // rotate entire body with lateral mouse movement
        } 
    }
}
