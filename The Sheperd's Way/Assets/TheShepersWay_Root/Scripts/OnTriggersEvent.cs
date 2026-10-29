using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggersEvent : MonoBehaviour
{
    [SerializeField] int secondsToWait = 1;
    float contadorTiempo;
    [SerializeField] float maxEscalaY = 1.1f;
    bool isPressed;
    [SerializeField] GameObject chargingSymbol;
    [SerializeField] GameObject collisionedObject = null;
    [SerializeField] int objectTimes;

    public UnityEvent OnTrigger; // Se verá en el inspector

    private void Update()
    {
        if (contadorTiempo >= secondsToWait)
        {
            OnTrigger.Invoke();
        }

        if (isPressed && objectTimes == 1) contadorTiempo += Time.deltaTime;
        else
        {
            if (contadorTiempo > 0)
            {
                contadorTiempo -= Time.deltaTime;
            }
            else contadorTiempo = 0;
        }
        if (chargingSymbol != null) chargingSymbol.transform.localScale = new Vector3(chargingSymbol.transform.localScale.x, Mathf.Clamp(contadorTiempo / secondsToWait, 0f, maxEscalaY), chargingSymbol.transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            isPressed = true;
        if (collisionedObject != collision.gameObject)
        {
            objectTimes = 1;
            collisionedObject = collision.gameObject;
        }
        else
        {
            objectTimes = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPressed = false;
        
        if (chargingSymbol != null)
        {
            chargingSymbol.transform.localScale = new Vector3(chargingSymbol.transform.localScale.x, 0f, chargingSymbol.transform.localScale.z);
        }
        if (objectTimes == 1) objectTimes = 0;
        else if(objectTimes == 2)
        {
            objectTimes = 0;
            collisionedObject = null;
        }
    }

}
