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
    [SerializeField] bool isFleeing;
    [SerializeField] bool wasHurt;
    [SerializeField] bool canAttack;
    [SerializeField] float attackRate = 2f;
    float nextAttackTime = 0f;

    Animator wolfAnim;
    Rigidbody2D wolfRb;
    // es mejor poner una variable de currentLife y otra de max??

    void Start()
    {
        wolfAnim = GetComponent<Animator>();
        wolfRb = GetComponent<Rigidbody2D>();
        InvokeRepeating("FindSheeps",0, 4f);
    }

    private void FixedUpdate()
    {
        if(!canAttack && !isFleeing)
        {
            if (nearbyPlayer != null) //si detecta a uno de los players
            {
                //calcular si aun asi hay una oveja más cerca del player
                transform.position = Vector2.MoveTowards(wolfRb.position, nearbyPlayer.position, wolfSpeed * Time.deltaTime);
                if (nearbyPlayer.position.x > transform.position.x && !isFacingRight) WolfFlip();
                else if (nearbyPlayer.position.x < transform.position.x && isFacingRight) WolfFlip();
                if (Vector2.Distance(transform.position, nearbyPlayer.position) <= wolfAttackRange) canAttack = true;
                else canAttack = false;
                
            }
            else
            {
                if (searchIsOver)
                {
                    if (closestSheep != null)
                    {
                        transform.position = Vector2.MoveTowards(wolfRb.position, closestSheep.position, wolfSpeed * Time.deltaTime);
                        if (closestSheep.position.x > transform.position.x && !isFacingRight) WolfFlip();
                        else if (closestSheep.position.x < transform.position.x && isFacingRight) WolfFlip();
                        if (Vector2.Distance(transform.position, closestSheep.position) <= wolfAttackRange) canAttack = true;
                        else canAttack = false;
                    }
                    else
                    {
                        transform.position = transform.position;
                        FindSheeps();
                    }
                }
            }
        }
        if (nearbyPlayer != null && !nearbyPlayer.GetComponent<Collider2D>().enabled)
        {
            nearbyPlayer = null;
            FindSheeps(); // Buscar una nueva oveja si no hay jugador
        }
        if (closestSheep != null && !closestSheep.GetComponent<Collider2D>().enabled)
        {
            closestSheep = null;
            FindSheeps(); // Buscar una nueva oveja si desaparece
        }

        if (Time.time >= nextAttackTime && canAttack)
        {
            Attack();
            canAttack = false;
            nextAttackTime = Time.time + 1f / attackRate;
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

    void Attack()
    {
        transform.position = transform.position;
        wolfAnim.SetTrigger("Attack");
    }
    void FindSheeps()
    {
        GameObject[] sheeps = GameObject.FindGameObjectsWithTag("Sheep");
        float closestSheepDistance = 50;

        searchIsOver = false;
        for (int i = 0; i < sheeps.Length; ++i)
        {
            float distanceToSheep = Vector2.Distance(sheeps[i].transform.position, wolfRb.transform.position);
            if (distanceToSheep < closestSheepDistance && sheeps[i].GetComponent<Collider2D>().enabled)
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
        StopAllCoroutines();
        wolfLife -= damage;
        wolfAnim.SetTrigger("Hurt");
        StartCoroutine(ResetHurt());
    }

    //Hacer que si de repente recibe mucho daño en pocos segundos que se aparte pa tras

    IEnumerator ResetHurt()
    {
        if (!wasHurt) wasHurt = true;
        else
        {
            // Dirección opuesta al jugador
            Vector2 direccionHuida = (transform.position - nearbyPlayer.position).normalized;

            isFleeing = true;
            // Elegimos una distancia aleatoria para la huida
            float distanciaHuida = Random.Range(4f, 7f);
            Vector2 puntoHuir = (Vector2)transform.position + (direccionHuida * distanciaHuida);

            StartCoroutine(RunToPoint(puntoHuir));
        }
        yield return new WaitForSeconds(2);
        wasHurt = false;
        isFleeing = false;
    }
    IEnumerator RunToPoint(Vector2 destino)
    {
        while (Vector2.Distance(transform.position, destino) > 0.1f) // Mientras no haya llegado
        {
            transform.position = Vector2.MoveTowards(transform.position, destino, wolfSpeed * Time.deltaTime);
            yield return null; // Esperar al siguiente frame
        }
    }

    void Death()
    {
        Debug.Log("Enemy died");
        wolfAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        wolfRb.isKinematic = true;
        this.enabled = false;
    }
}
