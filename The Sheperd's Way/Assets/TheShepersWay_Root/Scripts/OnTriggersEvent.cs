using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggersEvent : MonoBehaviour
{
    public UnityEvent OnTrigger; // Se verá en el inspector



    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTrigger.Invoke();
    }

}
