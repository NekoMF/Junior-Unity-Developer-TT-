using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float mouseSensitivity = 700f;

    float xRotation;
    float yRotation;
    public float topClamp = -90f;
    public float bottomClamp = 90f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; 

        xRotation -= mouseY;
        xRotation = Math.Clamp(xRotation,topClamp, bottomClamp);


        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f); 
    }
}
