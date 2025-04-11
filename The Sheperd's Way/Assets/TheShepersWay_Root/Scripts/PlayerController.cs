using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int shepherdLife = 100;
    [SerializeField] float speed;
    [SerializeField] bool isFacingRight;
    [SerializeField] bool isBurning;
    [SerializeField] bool canDie;
    Vector2 moveInput;
    Rigidbody2D rb;
    Animator shepherdAnim;

    [Header("Attack Variables")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int attackDamage = 10;
    [SerializeField] bool canAttack;
    [SerializeField] bool sheepHeld;
    [SerializeField] int canSheep; // 0 = no sheep near and none grabbed, 1 = sheep near, 2 = sheep grabbed

    [SerializeField] Image playerHealthBar;
    [SerializeField] GameObject heldSheep;
    [SerializeField] DogController player2;
    [SerializeField] float attackRate = 2f;
    float nextAttackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shepherdAnim = GetComponent<Animator>();
        heldSheep = null;
        canDie = true;

        //Temporal, despues hacer por colliders o mejorar
        player2 = GameObject.Find("P2").GetComponent<DogController>();


    }
    private void Update()
    {
        if(moveInput != null && moveInput.y == 0 && moveInput.x == 0 && shepherdAnim.GetBool("Walk") == true) { shepherdAnim.SetBool("Walk", false); }
        if (isBurning)
        {
            if (!IsInvoking("FireDamage")) InvokeRepeating("FireDamage", 0f, 1.5f);
        }
        else
        {
            if (IsInvoking("FireDamage")) CancelInvoke("FireDamage");
        }
        if (playerHealthBar != null) playerHealthBar.fillAmount = shepherdLife / 100f;
        if (shepherdLife<=0 && canDie) P1Death();
        else
        {

            if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight)) Flip();
            if (Time.time >= nextAttackTime && canAttack)
            {
                Attack();
                canAttack = false;
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        if (Vector2.Distance(transform.position, player2.transform.position) < 3)
        {
            player2.helpedByP1 = true;
        }
        else player2.helpedByP1 = false;
        // Seguramente acabe poniendo la P1Life aquí en vez del GameManager
    }
    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if(canSheep != 2 && canSheep!= 4) //Porque si está cerca y la registra se ralentiza
        {
            rb.velocity = moveInput * speed;
        }
        else rb.velocity = moveInput * (speed/2);
        if(shepherdAnim.GetBool("Walk") == false) shepherdAnim.SetBool("Walk", true);

    }
    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
    void Attack()
    {
        shepherdAnim.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            if(enemy is BoxCollider2D)
            {
                enemy.GetComponent<WolfAI>().TakeDamage(attackDamage);
                enemy.GetComponent<WolfAI>().nearbyPlayer = gameObject.transform;
                Debug.Log("You hit " + enemy.name);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep") && canSheep == 0)
        {
                canSheep = 1;
                heldSheep = collision.gameObject;
        }
        if (collision.CompareTag("Player") && canSheep == 0)
        {
            canSheep = 3;
            heldSheep = collision.gameObject;
            Debug.Log("Estás cerca del perro");
        }
        if (collision.gameObject.CompareTag("Weapon")) TakeDamage(10);
        if (collision.gameObject.CompareTag("Fire")) isBurning = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep") && !sheepHeld)
        {
            canSheep = 0;
            heldSheep = null;
        }
        if (collision.CompareTag("Player") && !sheepHeld)
        {
            canSheep = 0;
            heldSheep = null;
        }
        if (collision.gameObject.CompareTag("Fire")) isBurning = false;
    }


    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
            moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.performed) canAttack = true;
        if(context.canceled) canAttack = false;
    }
    public void OnGrabSheep(InputAction.CallbackContext context)
    {

        if (context.performed && canSheep == 1)
        {
            sheepHeld = true;
            if (shepherdAnim.GetBool("GrabSheep") == false) shepherdAnim.SetBool("GrabSheep", true);

            heldSheep.transform.SetParent(transform);
            heldSheep.SetActive(false);
            canSheep = 2;
        }
        else if (context.performed && canSheep == 2)
        {
            shepherdAnim.SetBool("GrabSheep", false);
            OnLeaveSheep();
        }
        else if (context.performed && canSheep == 3) //Pillar perro
        {
            sheepHeld = true;
            if(shepherdAnim.GetBool("GrabDog") == false) shepherdAnim.SetBool("GrabDog", true);
            heldSheep.GetComponent<DogController>().heldByP1 = true;
            heldSheep.transform.SetParent(transform);
            if (isFacingRight && !heldSheep.GetComponent<DogController>().isFacingRight) heldSheep.GetComponent<DogController>().DogFlip();
            else if (!isFacingRight && heldSheep.GetComponent<DogController>().isFacingRight) heldSheep.GetComponent<DogController>().DogFlip();
            heldSheep.GetComponent<SpriteRenderer>().enabled = false;
            heldSheep.GetComponent<BoxCollider2D>().enabled = false;
            canSheep = 4;
        }
        else if (context.performed && canSheep == 4)
        {
            heldSheep.GetComponent<DogController>().heldByP1 = false;
            heldSheep.GetComponent<BoxCollider2D>().enabled = true;
            shepherdAnim.SetBool("GrabDog", false);
            OnLeaveSheep();
        }
    }
    void OnLeaveSheep()
    {   
        if (heldSheep != null)
        {
            if(!heldSheep.activeSelf) //no se si va
            {
                heldSheep.SetActive(true);
            }
            if(!heldSheep.GetComponent<SpriteRenderer>().enabled)
            {
                heldSheep.GetComponent<SpriteRenderer>().enabled = true;
            }
            heldSheep.transform.SetParent(null);
            
            canSheep = 0;
            sheepHeld = false;
        }
    }

    #endregion

    public void TakeDamage(int damage)
    {
        shepherdLife -= damage;
        shepherdAnim.SetTrigger("Hurt");
    }
    void FireDamage()
    {
        TakeDamage(5);
    }
    void P1Death()
    {
        OnLeaveSheep();
        canDie = false;
        shepherdAnim.ResetTrigger("Hurt");
        Debug.Log("P1 died");
        //shepherdAnim.SetTrigger("Death");
        shepherdAnim.SetTrigger("Death");

        GetComponent<Collider2D>().enabled = false;
        //GameManager.Instance.currentGameState = GameState.gameOver;
        //this.enabled = false;
    }
}
