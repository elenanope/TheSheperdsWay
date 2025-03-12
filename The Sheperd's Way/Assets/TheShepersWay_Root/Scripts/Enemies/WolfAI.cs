using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAI : MonoBehaviour
{
    [SerializeField] int wolfLife = 50;
    Animator wolfAnim;
    // es mejor poner una variable de currentLife y otra de max??

    // Start is called before the first frame update
    void Start()
    {
        wolfAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(wolfLife <= 0)
        {
            //wolfLife = 0;
            
            Death();
        }
    }

    void SearchForVictims()
    {
        //Encuentra todas las ovejas y va a por la más cercana
        //Si en el camino se encuentra con el pastor/perro, cambia su focus a el otro hasta que lo mate o una oveja esté más cerca
    }

    public void TakeDamage(int damage)
    {
        wolfLife -= damage;
        wolfAnim.SetTrigger("Hurt");
    }

    //Hacer que si de repente recibe mucho daño en pocos segundos que se aparte pa tras

    void Death()
    {
        Debug.Log("Enemy died");
        wolfAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
