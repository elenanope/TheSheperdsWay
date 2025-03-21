using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepAI : MonoBehaviour
{
    [Header("Sheep Movement Parameters")]
    [SerializeField] int sheepLife = 30;
    [SerializeField] float sheepSpeed;

    public Transform objectToFollow;
    [SerializeField] float distanceBetween = 2f;
    FormationLeader leader;
    int direction = 1; // 1 = derecha, -1 = izquierda

    [SerializeField] float wanderingSpeed = 5;
    [SerializeField] bool isWandering;


    [Header("Enemy Detection Parameters")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float detectionRadius;
    Collider2D[] enemies;

    [Header("Sheep States")]
    public bool sheepInLine;
    [SerializeField] bool isFleeing;
    [SerializeField] bool isWalking;
    [SerializeField] bool isFacingRight = true;
    [SerializeField] bool sheepCanDie;
    //Autoreferences
    BoxCollider2D sheepCol;
    Rigidbody2D sheepRb;
    Animator sheepAnim;

    private void OnDisable()
    {
        StopFollowing();
        StopAllCoroutines();
        isWandering = false;
        isWalking = false;
        CancelInvoke("CheckForEnemies");
    }
    void Start()
    {
        leader = FindObjectOfType<FormationLeader>();
        sheepCol = GetComponent<BoxCollider2D>();
        sheepRb = GetComponent<Rigidbody2D>();
        sheepAnim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        sheepCanDie = true;
        InvokeRepeating("CheckForEnemies", 0f, 2f);
    }
    private void FixedUpdate()
    {
        if (!sheepInLine && !isFleeing)
        {
            if (!isWandering) StartCoroutine(Wander());
            float yOffset = Random.Range(-0.05f, 0.05f);
            if (isWalking)
            {
                sheepRb.velocity = new Vector2(direction * wanderingSpeed, sheepRb.velocity.y + yOffset);
            }
            else
            {
                sheepRb.velocity = new Vector2(0, 0);
            }
            //Hacer que todo esto sea un punto aleatorio a x distancia
        }
        else
        {
            StopCoroutine(Wander());
            isWandering = false;
        }
    }

    void Update()
    {
        if (sheepLife <= 0 && sheepCanDie) SheepDeath();
        if (sheepInLine) FollowingDog();
        if (leader.sheepsInLine.Count == 0) StopFollowing();

        if (isWalking) sheepAnim.SetBool("Walk", true);
        else sheepAnim.SetBool("Walk", false);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Sheep") && isWalking)
        {
            StopAllCoroutines(); //Provisional: despues poner que simplemnte hagan otro camino alrededor
            isWalking = false;
            isWandering = false;
            Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon")) TakeDamage(10);
    }
    void CheckForEnemies()
    {
        enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        if (enemies.Length > 0) FleeingFromEnemy();  //hacer que no huyan o no tanto si esta muerto
    }

    #region Sheep Behaviours
    public void Running(int barkZone)
    {
        //1 = up, 2 = right, 3 = down, 4 = left
        StopAllCoroutines();
        float xDirection;
        float yDirection;
        Vector2 puntoCorrer;
        isWandering = false;
        isFleeing = false;

        if (barkZone == 1)
        {
            xDirection = Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
            yDirection = Random.Range(transform.position.y + 0.3f, transform.position.y + 4);
            puntoCorrer = new Vector2(xDirection, yDirection);
        }

        else if (barkZone == 2)
        {
            xDirection = Random.Range(transform.position.x + 0.3f, transform.position.x + 4);
            yDirection = Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
            puntoCorrer = new Vector2(xDirection, yDirection);
            if(!isFacingRight) Flip();
        }

        else if (barkZone == 3)
        {
            xDirection = Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
            yDirection = Random.Range(transform.position.y - 4, transform.position.y - 0.3f);
            puntoCorrer = new Vector2(xDirection, yDirection);
        }

        else if (barkZone == 4)
        {
            xDirection = Random.Range(transform.position.x - 4, transform.position.x - 0.3f);
            yDirection = Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
            puntoCorrer = new Vector2(xDirection, yDirection);
            if (isFacingRight) Flip();
        }
        else return;
        StartCoroutine(RunToPoint(puntoCorrer));
    }

    void FollowingDog ()
    {
        if (objectToFollow != null)
        {
            float currentDistance = Vector2.Distance(transform.position, objectToFollow.position);

            if (currentDistance >= distanceBetween)
            {
                isWalking = true;
                transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, sheepSpeed * Time.deltaTime);
                if (transform.position.x > objectToFollow.position.x && isFacingRight) Flip();
                else if (transform.position.x < objectToFollow.position.x && !isFacingRight) Flip();
            }
            else 
            {
                transform.position = transform.position;
                isWalking = false;
            }
        }
    }

    void StopFollowing()
    {
        sheepInLine = false;
        objectToFollow = null;
        leader.RemoveSheep(transform);
    }

    void FleeingFromEnemy()
    {
        StopAllCoroutines();
            Vector2 fleeDirection = (transform.position - enemies[0].transform.position).normalized; 
            float fleeDistance = 3f; // Distancia a la que queremos que huya
            Vector2 fleePoint = (Vector2)transform.position + (fleeDirection * fleeDistance); // Calculamos un punto más alejado
            isWandering = false;
            isFleeing = true;
            StopFollowing();
            if (enemies[0].transform.position.x > transform.position.x && isFacingRight) Flip();
            else if (enemies[0].transform.position.x < transform.position.x && !isFacingRight) Flip();
            StartCoroutine(RunToPoint(fleePoint));
            //isWalking = true; //cambiar por velocidad fleeing y que sea más rápida?
    }
    
    void FleeingFromBattle() //opcional
    {

    }

    #endregion

    IEnumerator RunToPoint(Vector2 destino)
    {
        while (Vector2.Distance(transform.position, destino) > 0.1f) // Mientras no haya llegado
        {
            transform.position = Vector2.MoveTowards(transform.position, destino, sheepSpeed * Time.deltaTime);
            yield return null; // Esperar al siguiente frame
        }
    }

    IEnumerator Wander()
    {
        int walkWait = Random.Range(3, 11);
        int walkTime = Random.Range(0, 3);
        int flipWait = Random.Range(4, 8);
        bool flippingASheep = Random.Range(0, 2) == 0;

        isWandering = true;
        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(flipWait);
        if (flippingASheep) Flip();
        isWandering = false;
        //Meter tmb que pueda bajar la cabeza tipo ñam ñam hierba
    }

    void Flip()
    {
        direction *= -1;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
    public void TakeDamage(int damage)
    {
        sheepAnim.SetTrigger("Hurt");
        sheepLife -= damage;
    }
    void SheepDeath()
    {
        sheepCanDie = false;
        transform.position = transform.position;
        sheepRb.isKinematic = true;
        Debug.Log("A sheep died");
        sheepAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
