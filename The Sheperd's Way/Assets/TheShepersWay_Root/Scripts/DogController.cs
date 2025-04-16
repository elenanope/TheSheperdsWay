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
    [SerializeField] float dogSpeed = 6;
    public bool isFacingRight;
    [SerializeField] bool isBurning;
    public bool heldByP1;
    public bool isFainted;
    [SerializeField] float healingTime = 10;
    [SerializeField] float timePassed;
    [SerializeField] float detectionRadius = 3.7f;

    public bool helpedByP1;
    [SerializeField] bool canBark1;
    [SerializeField] int lastDirection; // 1 up, 2 right, 3 down, 4 left
    [SerializeField] LayerMask sheepsLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float barkRate = 1f;
    float nextBarkTime = 0f;
    public bool bark2;

    [SerializeField] Image playerHealthBar;
    [SerializeField] GameObject[] barkDirectionUI;
    [SerializeField] GameObject vfx;
    [SerializeField] GameObject vfxDust;
    [SerializeField] GameObject uiBark2;
    [SerializeField]BoxCollider2D boxColDog;
    CircleCollider2D circleColDog;

    // Start is called before the first frame update
    void Start()
    {
        dogRb = GetComponent<Rigidbody2D>();
        dogAnim = GetComponent<Animator>();
        circleColDog = GetComponent<CircleCollider2D>();
        boxColDog = GetComponent<BoxCollider2D>();
        lastDirection = 2;
    }
    private void FixedUpdate()
    {
        if(!isFainted) Move();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Weapon")) TakeDamage(10);
        if (collision.gameObject.CompareTag("Fire") && !isFainted) isBurning = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fire")) isBurning = false;
    }
    void Update()
    {
        if(uiBark2 != null)
        {
            if (bark2 && !uiBark2.activeSelf) uiBark2.SetActive(true);
            else if (!bark2 && uiBark2.activeSelf) uiBark2.SetActive(false);
        }
        
        if(!gameObject.GetComponent<SpriteRenderer>().enabled) vfxDust.SetActive(false);
        else vfxDust.SetActive(true);
        if(moveInput != null && moveInput.x == 0 && moveInput.y  == 0) 
        {
            dogAnim.SetBool("Walk", false);
        }
        if (isBurning)
        {
            if (!IsInvoking("FireDamage")) InvokeRepeating("FireDamage", 0f, 1.5f);
        }
        else
        {
            if (IsInvoking("FireDamage")) CancelInvoke("FireDamage");
        }
        if (transform.localScale.x < 0) isFacingRight = false;
        if (heldByP1)
        {
            dogRb.isKinematic = true;
            dogAnim.SetBool("Held", true);
        }
        if (!heldByP1)
        {
            if(dogAnim.GetBool("Held")) dogAnim.SetBool("Held", false);
            if (dogRb.isKinematic) dogRb.isKinematic = false;
        }
            
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
            dogLife = 50;
            dogAnim.SetBool("Fainted", false);
            boxColDog.enabled = true;
            circleColDog.enabled = true;
            timePassed = 0;
            if (vfx.activeSelf) vfx.SetActive(false);
        }

        if (!isFainted)// Hacer que no pueda hacer NADA
        {
            if (dogLife <= 0) Faint();

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
        boxColDog.enabled = false;
        Debug.Log("Se desaciva el collider!!!!");
        circleColDog.enabled = false;
    }

    void Move()
    {
        if(!heldByP1)
        {
            dogAnim.SetBool("Walk", true);
            dogRb.velocity = moveInput * dogSpeed;
        }

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
            SheepAI sheepAi = sheep.GetComponent<SheepAI>();
            if(sheep != null && !sheepAi.sheepInLine) sheepAi.Running(lastDirection);
        }

        foreach (Collider2D enemy in enemies)
        {
            if(enemy != null) enemy.gameObject.GetComponent<TakeStun>().TakePause();
        }
    }

    #region Input Methods

    public void OnMoveDog(InputAction.CallbackContext context)
    {
        int canShowUI = 0;
        moveInput = context.ReadValue<Vector2>();
        for (int i = 0; i < barkDirectionUI.Length; i++)
        {
            if (barkDirectionUI[i] != null) canShowUI++;
        }

            if (canShowUI >=4)
            {
                if (moveInput.y > 0)
                {
                    lastDirection = 1;
                    Debug.Log("Up pressed");
                    barkDirectionUI[0].SetActive(true);
                    barkDirectionUI[1].SetActive(false);
                    barkDirectionUI[2].SetActive(false);
                    barkDirectionUI[3].SetActive(false);
                }
                else if (moveInput.y < 0)
                {
                    lastDirection = 3;
                    Debug.Log("Down pressed");
                    barkDirectionUI[2].SetActive(true);
                    barkDirectionUI[1].SetActive(false);
                    barkDirectionUI[3].SetActive(false);
                    barkDirectionUI[0].SetActive(false);
                }

                if (moveInput.x > 0)
                {
                    lastDirection = 2;
                    Debug.Log("Right pressed");
                    barkDirectionUI[1].SetActive(true);
                    barkDirectionUI[2].SetActive(false);
                    barkDirectionUI[0].SetActive(false);
                    barkDirectionUI[3].SetActive(false);
                }
                else if (moveInput.x < 0)
                {
                    lastDirection = 4;
                    Debug.Log("Left pressed");
                    barkDirectionUI[3].SetActive(true);
                    barkDirectionUI[1].SetActive(false);
                    barkDirectionUI[2].SetActive(false);
                    barkDirectionUI[0].SetActive(false);
                }
        }
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
    void FireDamage()
    {
        TakeDamage(5);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(heldByP1) Gizmos.DrawWireSphere(transform.position, detectionRadius +2);
        else Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
