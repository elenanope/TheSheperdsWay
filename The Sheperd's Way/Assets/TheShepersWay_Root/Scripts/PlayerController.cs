using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class PlayerController : MonoBehaviour
{
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
        if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight)) Flip();
        if (Time.time >= nextAttackTime && canAttack)
        {
            Attack();
            canAttack = false;
            nextAttackTime = Time.time + 1f / attackRate;
        }
        if(GameManager.Instance != null) if (GameManager.Instance.totalLife <= 0) P1Death();
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
            enemy.GetComponent<WolfAI>().TakeDamage(attackDamage);
            Debug.Log("You hit " + enemy.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sheep"))
        {
            canSheep = 1;
            heldSheep = collision.gameObject; 
            

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
        if (context.performed)
        {
            if (canSheep == 1)
            {
                shepherdAnim.SetBool("GrabSheep", true);
                heldSheep.transform.SetParent(transform);
                heldSheep.SetActive(false);
                canSheep = 2;
            }
            else Debug.Log("You are currently holding a sheep");
        }
        if (context.canceled)
        {
            if (canSheep != 2)
            {
                canSheep = 0;
            }
        }
            
        
            //if (context.performed) canSheep = 1;
            //if (context.canceled) canSheep = 0;
        
        //else Debug.Log("You are currently holding a sheep");
        
    }
    public void OnLeaveSheep(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (canSheep == 2)
            {
                heldSheep.SetActive(true);
                heldSheep.transform.SetParent(null);
                shepherdAnim.SetBool("GrabSheep", false);
                canSheep = 0;
            }
            else Debug.Log("NOT currently holding a sheep");
        }
        
    }

    #endregion


    void GrabTheSheep()
    {

    }
    void P1Death()
    {
        Debug.Log("P1 died");
        shepherdAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.currentGameState = GameState.gameOver;
        this.enabled = false;
    }
}
