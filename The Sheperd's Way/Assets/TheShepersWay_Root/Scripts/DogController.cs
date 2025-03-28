using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DogController : MonoBehaviour
{

    Vector2 moveInput;
    Rigidbody2D dogRb;
    Animator dogAnim;
    [SerializeField] int dogLife = 50;
    [SerializeField] float dogSpeed;
    public bool isFacingRight;
    public bool heldByP1;
    public bool isFainted;
    [SerializeField] float healingTime = 10;
    [SerializeField] float timePassed;
    [SerializeField] float detectionRadius;

    public bool helpedByP1;
    [SerializeField] bool canBark1;
    [SerializeField] LayerMask sheepsLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float barkRate = 1f;
    float nextBarkTime = 0f;
    public bool bark2;
    [SerializeField] float distance;
    [SerializeField] float barkForce;

    [SerializeField] Image playerHealthBar;
    [SerializeField] GameObject vfx;
    BoxCollider2D[] boxColDog;

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
        if (heldByP1) dogRb.isKinematic = true;
        if (!heldByP1 && dogRb.isKinematic) dogRb.isKinematic = false;
        if (playerHealthBar != null)
        {
            
            if (isFainted)
            {
                playerHealthBar.color = HexToColor("#F1FF00", 210/255f);
                playerHealthBar.fillAmount = timePassed / healingTime;
            }
            else
            {
                playerHealthBar.color = HexToColor("#78800E", 255f);
                playerHealthBar.fillAmount = dogLife / 50f;
            }

        }
        if (isFainted)
        {
            if (helpedByP1)
            {
                timePassed += Time.deltaTime * 2; //arreglar
                if (vfx != null) vfx.SetActive(true);
            }
            else timePassed += Time.deltaTime; //arreglar
            //Seguramente cambiar esto
        }

        if (timePassed >= healingTime)
        {
            isFainted = false;
            dogLife = 100;
            dogAnim.SetBool("Fainted", false);
            foreach (BoxCollider2D col in boxColDog) col.enabled = true;
            timePassed = 0;
            if (vfx.activeSelf) vfx.SetActive(false);
        }

        if (!isFainted)// Hacer que no pueda hacer NADA
        {
            if (dogLife <= 0)
            {
                Faint();
            }

            if(!heldByP1)
            {
                if (moveInput.x > 0 && !isFacingRight) DogFlip();
                else if (moveInput.x < 0 && isFacingRight) DogFlip();
            }
            
            if (Time.time >= nextBarkTime && canBark1)
            {
                Bark1();
                canBark1 = false;
                nextBarkTime = Time.time + 1f / barkRate;
            }
        }
    }
    // Método para convertir hexadecimal a Color
    Color HexToColor(string hex, float alpha = 1f)
    {
        // Eliminar el símbolo # si está presente
        hex = hex.Replace("#", "");

        // Convertir los valores hexadecimales a RGB
        float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
        float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
        float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;

        return new Color(r, g, b, alpha);
    }
    void Faint()
    {
        dogAnim.ResetTrigger("Hurt");
        dogLife = 0;
        isFainted = true;
        dogAnim.SetBool("Fainted", true);
        dogAnim.SetTrigger("Faints"); // el anim aqui hace el tonto
        boxColDog = gameObject.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D col in boxColDog) col.enabled = false;
    }

    void Move()
    {
        if(!heldByP1) dogRb.velocity = moveInput * dogSpeed;

    }
    public void DogFlip()
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
        Collider2D[] enemies;
        if (!heldByP1) enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        else enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius + 2, enemyLayer);

        foreach (Collider2D sheep in sheeps)
        {
            if(sheep != null)
            {
                //Hacer que simplemente se muevan en contra de él?
                if(isFacingRight) sheep.gameObject.GetComponent<SheepAI>().Running(2);
                else sheep.gameObject.GetComponent<SheepAI>().Running(4);
            }
        }

        foreach (Collider2D enemy in enemies)
        {
            if(enemy != null)
            {
                enemy.gameObject.GetComponent<TakeStun>().TakePause();
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
    {//El perro tmb recibe daño aunque esté heldByP1
        dogLife -= damage;
        dogAnim.SetTrigger("Hurt");
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(heldByP1) Gizmos.DrawWireSphere(transform.position, detectionRadius +2);
        else Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
