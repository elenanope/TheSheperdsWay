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

    public void TakeDamage(int damage)
    {
        wolfLife -= damage;
        wolfAnim.SetTrigger("Hurt");
    }

    void Death()
    {
        Debug.Log("Enemy died");
        wolfAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
