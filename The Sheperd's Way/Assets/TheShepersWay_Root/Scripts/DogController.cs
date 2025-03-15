using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{

    Vector2 moveInput;
    Rigidbody2D dogRb;
    Animator dogAnim;
    [SerializeField] int dogLife = 50;
    [SerializeField] float dogSpeed;
    [SerializeField] bool isFacingRight;
    public bool isFainted;
    [SerializeField] float healingTime = 5;
    [SerializeField] float timePassed;
    [SerializeField] float detectionRadius;

    [SerializeField] bool canBark1;
    [SerializeField] LayerMask sheepsLayer;
    [SerializeField] float barkRate = 1f;
    float nextBarkTime = 0f;
    public bool bark2;
    [SerializeField] float distance;
    [SerializeField] float barkForce;

    // Start is called before the first frame update
    void Start()
    {
        dogRb = GetComponent<Rigidbody2D>();
        dogAnim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if(!isFainted) Move();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon")) TakeDamage(10);
    }
    void Update()
    {
        if (isFainted)
        {
            timePassed += Time.deltaTime; //arreglar
        }
        if (timePassed >= healingTime)
        {
            isFainted = false;
            dogLife = 100;
            dogAnim.SetBool("Fainted", false);
            gameObject.GetComponent<Collider2D>().enabled = true;
            timePassed = 0;
        }
        
        if (!isFainted)// Hacer que no pueda hacer NADA
        {
            if (dogLife <= 0)
            {
                Faint();
            }

            if (moveInput.x > 0 && !isFacingRight) DogFlip();
            else if (moveInput.x < 0 && isFacingRight) DogFlip();

            if (Time.time >= nextBarkTime && canBark1)
            {
                Bark1();
                canBark1 = false;
                nextBarkTime = Time.time + 1f / barkRate;
            }
        }
    }
    void Faint()
    {
        dogAnim.ResetTrigger("Hurt");
        dogLife = 0;
        isFainted = true;
        dogAnim.SetBool("Fainted", true);
        dogAnim.SetTrigger("Faints"); // el anim aqui y en el player 1 hace el tonto
        gameObject.GetComponent<Collider2D>().enabled = false;
    }
    void Move()
    {
        dogRb.velocity = moveInput * dogSpeed;
    }
    void DogFlip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
    void Bark1()
    {
        dogAnim.SetTrigger("Bark1");
        Collider2D[] sheeps = Physics2D.OverlapCircleAll(transform.position, detectionRadius, sheepsLayer);
        foreach (Collider2D sheep in sheeps)
        {
            if(sheep != null)
            {
                if(isFacingRight) sheep.gameObject.GetComponent<SheepAI>().Running(2);
                else sheep.gameObject.GetComponent<SheepAI>().Running(4);
            }
        }

        /*
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right * transform.localScale.x, rayDistance);
        Debug.DrawRay(transform.position, transform.right * transform.localScale.x * rayDistance, Color.yellow);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Debug.Log("Ray hit: " + hit.collider.gameObject.name);
                if (hit.collider.CompareTag("Sheep"))
                {
                    //Cambiar para que las ovejas hagan un MoveTowards ese punto, de esa manera pueden parar si alguien les ataca por ejemplo
                    Rigidbody2D hitRb = hit.collider.GetComponent<Rigidbody2D>();
                }
            }
        }
        */
    }

    #region Input Methods

    public void OnMoveDog(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnBark1(InputAction.CallbackContext context)
    {
        if(context.performed) canBark1 = true;
    }
    public void OnBark2(InputAction.CallbackContext context)
    {
        if(context.performed) bark2 = !bark2;
    }

    #endregion

    public void TakeDamage(int damage)
    {
        dogLife -= damage;
        dogAnim.SetTrigger("Hurt");

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
