using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAI : MonoBehaviour
{
    [Header("Wolf Movement Parameters")]
    [SerializeField] int wolfLife = 50;
    [SerializeField] float wolfSpeed = 3;

    [Header("Wolf Attack Parameters")]
    [SerializeField] float wolfAttackRange = 1.5f;
    [SerializeField] Transform closestSheep = null;
    public Transform nearbyPlayer = null;
    [SerializeField] bool playerInRange;
    [SerializeField] float persecutionTime = 7f;
    [SerializeField] float persecutionTimePassed;
    [SerializeField] float attackRate = 2f;
    [SerializeField] float rayDistance = 8f;
    float nextAttackTime = 0f;

    [Header("Wolf States")]
    [SerializeField] bool objectDetected;
    [SerializeField] bool attackedByPlayer;
    [SerializeField] bool searchIsOver;
    [SerializeField] bool isFacingRight;
    [SerializeField] bool isFleeing;
    [SerializeField] bool wasHurt;
    [SerializeField] bool canAttack;
    [SerializeField] bool canDie;

    [SerializeField] Transform sensor;

    Animator wolfAnim;
    Rigidbody2D wolfRb;
    // es mejor poner una variable de currentLife y otra de max??

    void Start()
    {
        wolfAnim = GetComponent<Animator>();
        wolfRb = GetComponent<Rigidbody2D>();
        canDie = true;
        InvokeRepeating("FindSheeps",0, 4f);
    }

    private void FixedUpdate()
    { //añadirle tipo patrol/wander
        
        if (!canAttack && !isFleeing)
        {
            StopAllCoroutines();
            if (nearbyPlayer != null)
            {

                if ((playerInRange && Vector2.Distance(transform.position, nearbyPlayer.position) < 20) || attackedByPlayer)
                {
                    StartCoroutine(RunToPoint(nearbyPlayer.position));
                    if (Vector2.Distance(transform.position, nearbyPlayer.position) <= wolfAttackRange) canAttack = true;
                    else canAttack = false;
                }
                else nearbyPlayer = null;
            }
            else
            {
                if (searchIsOver)
                {
                    if (closestSheep != null && Vector2.Distance(transform.position, closestSheep.position) <= 5 || objectDetected)
                    {
                        StartCoroutine(RunToPoint(closestSheep.position));
                        if (Vector2.Distance(transform.position, closestSheep.position) <= wolfAttackRange) canAttack = true;
                        else canAttack = false;
                    }
                    else
                    {
                        transform.position = transform.position;
                        FindSheeps();
                    }
                    RaycastHit2D[] hits = Physics2D.RaycastAll(sensor.position, -sensor.right, rayDistance); //error de ref?
                    Debug.DrawRay(sensor.position, -sensor.right * rayDistance, Color.blue);
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null)
                        {
                            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Sheep"))
                            {
                                Debug.Log("Objeto detectado: " + hit.collider.name);
                                objectDetected = true;
                            }
                        } //despues poner que si despues de x tiempo no detecta nada, que vuelva a object detected = false
                    }
                }
            }
        }
        if (nearbyPlayer != null && !nearbyPlayer.GetComponent<BoxCollider2D>().enabled)
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
            StopAllCoroutines();
            Attack();
            canAttack = false;
            nextAttackTime = Time.time + 1f / attackRate;
        }

    }
    
    void Update()
    {
        if(wolfLife <= 0 && canDie) Death();//wolfLife = 0;
        if (attackedByPlayer)
        {
            persecutionTimePassed += Time.deltaTime;
            if (persecutionTimePassed > persecutionTime) attackedByPlayer = false;
        }
    }

    #region Player In Range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) playerInRange = true; 
        //Que solo pille boxcollider, y ponerle otro al perro para qque cuando este aturdido solo se le desactive uno y el perro pueda ser curado
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) playerInRange = false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && nearbyPlayer == null)
        {
            playerInRange = true;
            nearbyPlayer = collision.gameObject.transform;
        }
    }
    #endregion

    #region Actions
    void Attack()
    {
        //transform.position = transform.position;
        wolfAnim.SetTrigger("Attack"); //Aqui mientras patina algo hacia ti, quitar si eso
        wolfRb.velocity = Vector2.zero;
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
        persecutionTimePassed = 0;
        attackedByPlayer = true;
        wolfLife -= damage;
        wolfAnim.SetTrigger("Hurt");
        StartCoroutine(ResetHurt());
    }
    void Death()
    {
        canDie = false;
        transform.position = transform.position;
        Debug.Log("Enemy died");
        wolfAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
    #endregion

    IEnumerator ResetHurt()
    {
        if (!wasHurt) wasHurt = true;
        else
        {
            Vector2 direccionHuida = (transform.position - nearbyPlayer.position).normalized;
            isFleeing = true;
            attackedByPlayer = false;
            float distanciaHuida = Random.Range(3f, 5f);
            Vector2 puntoHuir = (Vector2)transform.position + (direccionHuida * distanciaHuida);
            StartCoroutine(RunToPoint(puntoHuir));
        }
        yield return new WaitForSeconds(1);
        if (wasHurt)
        {
            wasHurt = false;  // Solo resetea si no ha vuelto a ser herido durante el tiempo
            isFleeing = false;
        }
    }
    IEnumerator RunToPoint(Vector2 destino)
    {
        while (Vector2.Distance(transform.position, destino) > 0.1f) // Mientras no haya llegado
        {
            transform.position = Vector2.MoveTowards(transform.position, destino, wolfSpeed * Time.deltaTime);
            if (destino.x > transform.position.x && !isFacingRight) WolfFlip();
            else if (destino.x < transform.position.x && isFacingRight) WolfFlip();
            yield return null; // Esperar al siguiente frame
        }
        if (Vector2.Distance(transform.position, destino) < 0.1f && isFleeing) isFleeing = false;
    }

}
