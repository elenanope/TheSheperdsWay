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

    public UnityEvent OnTrigger; // Se verá en el inspector

    private void Update()
    {
        if (contadorTiempo >= secondsToWait)
        {
            OnTrigger.Invoke();
            contadorTiempo = 0f;
            isPressed = false;
        }

        if (isPressed) contadorTiempo += Time.deltaTime;
        if (chargingSymbol != null) chargingSymbol.transform.localScale = new Vector3(chargingSymbol.transform.localScale.x, Mathf.Clamp(contadorTiempo / secondsToWait, 0f, maxEscalaY), chargingSymbol.transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        contadorTiempo = 0f;
        isPressed = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        contadorTiempo = 0f;
        isPressed = false;
        if (chargingSymbol != null)
        {
            chargingSymbol.transform.localScale = new Vector3(chargingSymbol.transform.localScale.x, 0f, chargingSymbol.transform.localScale.z);
        }
        //me da error  chargingSymbol.SetActive(false);
    }

}
