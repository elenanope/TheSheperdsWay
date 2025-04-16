using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] GameObject particles;

    Animator wolfAnim;
    SpriteRenderer wolfRenderer;
    Rigidbody2D wolfRb;
    float waitingTime = 10;
    float currentWaitingTime;

    private void Start()
    {
        wolfAI = GetComponent<WolfAI>();
        wolfAnim = GetComponent<Animator>();
        wolfRenderer = GetComponent<SpriteRenderer>();
        wolfRb = GetComponent<Rigidbody2D>();
    }
    public void TakePause()
    {
        if (!stunned && !ignoringStun)
        {
            wolfAI.StopAllCoroutines();
            wolfAI.enabled = false;
            stunned = true;
            //velocity 0 new
            gameObject.GetComponent<NavMeshAgent>().SetDestination(transform.position);
            wolfRb.bodyType = RigidbodyType2D.Static;
            //wolfAnim.SetBool("Stun", true);
            particles.SetActive(true);
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
                gameObject.GetComponent<NavMeshAgent>().enabled = true;
                //wolfAnim.SetBool("Stun", false);
                particles.SetActive(false);
                timeStunnedPassed = 0;
                wolfAI.enabled = true;
                wolfRb.bodyType = RigidbodyType2D.Dynamic;
                stunned = false;
            }
        }
        
        //Mide si sigues ladrando
        if (barkedTimes >= 1 && !ignoringStun)
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
