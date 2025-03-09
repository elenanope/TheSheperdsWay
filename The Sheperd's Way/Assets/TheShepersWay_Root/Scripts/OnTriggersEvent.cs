using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggersEvent : MonoBehaviour
{
    [SerializeField] int secondsToWait = 1;
    float contadorTiempo;
    bool isPressed;

    public UnityEvent OnTrigger; // Se verá en el inspector

    private void Update()
    {
        if(contadorTiempo >= secondsToWait) OnTrigger.Invoke();

        if (isPressed) contadorTiempo += Time.deltaTime;
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
    }


}
