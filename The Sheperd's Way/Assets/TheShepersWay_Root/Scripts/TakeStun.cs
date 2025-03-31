using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeStun : MonoBehaviour
{
    [SerializeField] float resetCautionTime = 18;
    [SerializeField] float cautionTime;
    [SerializeField] int barkedTimes;
    [SerializeField] float timeStunnedPassed;
    [SerializeField] float stunningTime = 2f;
    [SerializeField] bool stunned;
    [SerializeField] bool ignoringStun;
    [SerializeField] WolfAI wolfAI;

    Animator wolfAnim;
    float waitingTime = 10;
    float currentWaitingTime;

    private void Start()
    {
        wolfAI = GetComponent<WolfAI>();
        wolfAnim = GetComponent<Animator>();
    }
    public void TakePause()
    {
        if (!stunned && !ignoringStun)
        {
            wolfAI.enabled = false;
            stunned = true;
            wolfAnim.SetBool("Stun", true);
            barkedTimes += 1;
        }
        else Debug.Log("Enemy is already stunned/is used to it");
    }
    private void Update()
    {
        //Stunning
        if (stunned)
        {
            timeStunnedPassed += Time.deltaTime;
            if (timeStunnedPassed >= stunningTime) //sino, sacar
            {
                wolfAnim.SetBool("Stun", false);
                timeStunnedPassed = 0;
                wolfAI.enabled = true;
                stunned = false;
            }
        }
        

        //Mide si sigues ladrando
        if(barkedTimes >= 1 && !ignoringStun)
        {
            cautionTime += Time.deltaTime;
        }
        if(barkedTimes>=5)
        {
            if(cautionTime < resetCautionTime)
            {
                ignoringStun = true;
                Debug.Log("Se va a ignorar el ladrido");
            }
        }
        //si no te pasas está OK
        if(cautionTime >= resetCautionTime && barkedTimes > 0 && !ignoringStun)
        {
            cautionTime = 0;
            barkedTimes = 0;
            Debug.Log("Veces ladradas reseteadas");
        }

        //si te has pasado...
        if(ignoringStun)
        {
            currentWaitingTime += Time.deltaTime;
        }
        else if (currentWaitingTime>= waitingTime)
        {
            ignoringStun = false;
            currentWaitingTime = 0;
            cautionTime = 0;
            barkedTimes = 0;
            Debug.Log("Se puede volver a aturdir al enemigo");
        }
    }
}
