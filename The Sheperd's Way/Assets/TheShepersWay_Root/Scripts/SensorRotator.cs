using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorRotator : MonoBehaviour
{
    [SerializeField] bool inMaximum;
    [SerializeField] float turnSpeed = 25f;
    void Update()
    {
        float zRotation = transform.eulerAngles.z;
        if (zRotation > 180f)
        {
            zRotation -= 360f;  // Convertir a rango de -180 a 180 grados
        }

        //que rote en z de -55 a 55 y viceversa
        if (!inMaximum)
        {
            transform.Rotate(0, 0, turnSpeed * Time.deltaTime);
        }
        if(zRotation >= 55f) inMaximum = true;
        if(inMaximum)
        {
            transform.Rotate(0, 0, -turnSpeed * Time.deltaTime);
        }
        if (zRotation <= -55f) inMaximum = false;

    }
}
