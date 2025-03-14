using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAI : MonoBehaviour
{
    [SerializeField] int wolfLife = 50;
    [SerializeField] int wolfDamage = 10;
    [SerializeField] float wolfSpeed = 3;
    [SerializeField] float wolfAttackRange = 1.5f;
    [SerializeField] Transform closestSheep = null;
    [SerializeField] Transform nearbyPlayer = null;
    [SerializeField] bool searchIsOver;
    [SerializeField] bool isFacingRight;

    Animator wolfAnim;
    Rigidbody2D wolfRb;
    // es mejor poner una variable de currentLife y otra de max??

    void Start()
    {
        wolfAnim = GetComponent<Animator>();
        wolfRb = GetComponent<Rigidbody2D>();
        FindSheeps();
    }

    private void FixedUpdate()
    {
        if (nearbyPlayer != null) //si detecta a uno de los players
        {
            //calcular si aun asi hay una oveja más cerca del player
            transform.position = Vector2.MoveTowards(wolfRb.position, nearbyPlayer.position, wolfSpeed * Time.deltaTime);
            if (nearbyPlayer.position.x > transform.position.x && !isFacingRight) WolfFlip();
            else if (nearbyPlayer.position.x < transform.position.x && isFacingRight) WolfFlip();
            if (Vector2.Distance(transform.position, nearbyPlayer.position) <= wolfAttackRange) Attack();
        }
        else
        {
            if (searchIsOver)
            {
                if (closestSheep.gameObject.activeSelf)
                {
                    transform.position = Vector2.MoveTowards(wolfRb.position, closestSheep.position, wolfSpeed * Time.deltaTime);
                    if (closestSheep.position.x > transform.position.x && !isFacingRight) WolfFlip();
                    else if(closestSheep.position.x < transform.position.x && isFacingRight) WolfFlip();
                    if(Vector2.Distance(transform.position, closestSheep.position) <= wolfAttackRange) Attack();
                }
                else
                {
                    transform.position = transform.position;
                    FindSheeps();
                }
            }
        }
    }
    
    void Update()
    {
        if(wolfLife <= 0) Death();//wolfLife = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) nearbyPlayer = collision.gameObject.transform; // o poner que si se va bastante lejos sí vaya a por una oveja
    }
    /*
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == nearbyPlayer) nearbyPlayer = null;
    }
    */

    void Attack()
    {
        wolfAnim.SetTrigger("Attack");
    }
    void FindSheeps()
    {
        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        float closestSheepDistance = 30;

        closestSheep = null;
        searchIsOver = false;
        for (int i = 0; i < sheeps.Length; ++i)
        {
            float distanceToSheep = Vector2.Distance(sheeps[i].transform.position, wolfRb.transform.position);
            if (distanceToSheep < closestSheepDistance)
            {
                closestSheepDistance = distanceToSheep;
                closestSheep = sheeps[i].transform;
            }
        }
        searchIsOver = true;
    }

    //TakeAturdir

    void WolfFlip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
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
        wolfRb.isKinematic = true;
        this.enabled = false;
    }
}
