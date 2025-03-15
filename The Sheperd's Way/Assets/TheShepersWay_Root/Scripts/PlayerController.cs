using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int shepherdLife = 100;
    [SerializeField] float speed;
    [SerializeField] bool isFacingRight;
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

    [SerializeField] GameObject heldSheep;
    [SerializeField] float attackRate = 2f;
    float nextAttackTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shepherdAnim = GetComponent<Animator>();
        heldSheep = null;
    }
    private void Update()
    {
        if(shepherdLife<=0) P1Death();
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
        // Seguramente acabe poniendo la P1Life aquí en vez del GameManager
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.velocity = moveInput * speed;
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
        if (collision.gameObject.CompareTag("Weapon")) TakeDamage(10);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep") && !sheepHeld)
        {
            canSheep = 0;
            heldSheep = null;
        }
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
            shepherdAnim.SetBool("GrabSheep", true);
            heldSheep.transform.SetParent(transform);
            heldSheep.SetActive(false);
            canSheep = 2;
        }
            
        
            //if (context.performed) canSheep = 1;
            //if (context.canceled) canSheep = 0;
        
        //else Debug.Log("You are currently holding a sheep");
        
    }
    public void OnLeaveSheep(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (heldSheep != null)
            {
                heldSheep.SetActive(true);
                heldSheep.transform.SetParent(null);
                shepherdAnim.SetBool("GrabSheep", false);
                canSheep = 0;
                sheepHeld = false;
            }
            else Debug.Log("NOT currently holding a sheep");
        }
        
    }

    #endregion

    public void TakeDamage(int damage)
    {
        shepherdLife -= damage;
        shepherdAnim.SetTrigger("Hurt");
    }
    void GrabTheSheep()
    {

    }
    void P1Death()
    {
        shepherdAnim.ResetTrigger("Hurt");
        Debug.Log("P1 died");
        //shepherdAnim.SetTrigger("Death");
        shepherdAnim.Play("P1_Death");

        GetComponent<Collider2D>().enabled = false;
        //GameManager.Instance.currentGameState = GameState.gameOver;
        //this.enabled = false;
    }
}
