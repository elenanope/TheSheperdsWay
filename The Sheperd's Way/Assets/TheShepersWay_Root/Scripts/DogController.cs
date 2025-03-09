using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    [SerializeField] float dogSpeed;
    public bool isFainted;
    [SerializeField] bool canBark1;
    [SerializeField] bool isFacingRight;
    Vector2 moveInput;
    Rigidbody2D dogRb;
    [SerializeField] float distance;
    [SerializeField] float barkForce;
    [SerializeField] float frenoOvejas =2f;

    // Start is called before the first frame update
    void Start()
    {
        dogRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }
    public float sheepPushDistance = 5f; // How far the object will move toward the hit point
    float rayDistance = 10f; // Ajusta la distancia del rayo
    void Update()
    {
        if (moveInput.x > 0 && !isFacingRight) DogFlip();
        else if (moveInput.x < 0 && isFacingRight) DogFlip();

        if (canBark1)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, rayDistance) ;
            Debug.DrawRay(transform.position, transform.right * rayDistance, Color.yellow);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    Debug.Log("Ray hit: " + hit.collider.gameObject.name);
                    if (hit.collider.CompareTag("Sheep"))
                    {
                        //Cambiar para que las ovejas hagan un MoveTowards ese punto, de esa manera pueden parar si alguien les ataca por ejemplo
                        Rigidbody2D hitRb = hit.collider.GetComponent<Rigidbody2D>();
                        
                        //Si es posible hacer que se desvíen minimamente o que haya posibilidad de ello

                        //Activar bool en script oveja respectivo, que se mueva cierta distancia en un move towards(transform.position, transform.position + 5f, ...)
                        hitRb.AddForce(transform.right * rayDistance, ForceMode2D.Impulse);
                        hitRb.drag = frenoOvejas; // Establece un valor de drag para que se frene
                    }
                }
            }
            canBark1 = false;
        }
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

    #region Input Methods

    public void OnMoveDog(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnBark1(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            canBark1 = true;
            
            Debug.Log("You barked!");
        }
    }
    public void OnBark2(InputAction.CallbackContext context)
    {

    }

    #endregion
}
