using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{

    Vector2 moveInput;
    Rigidbody2D dogRb;
    Animator dogAnim;
    [SerializeField] int dogLife = 100;
    [SerializeField] float dogSpeed;
    [SerializeField] bool isFacingRight;
    public bool isFainted;
    [SerializeField] float healingTime = 5;
    [SerializeField] float timePassed;

    [SerializeField] bool canBark1;
    [SerializeField] float distance;
    [SerializeField] float barkForce;
    [SerializeField] float frenoOvejas =2f;

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
    //[SerializeField] float sheepPushDistance = 5f; // How far the object will move toward the hit point
    float rayDistance = 10f; // Ajusta la distancia del rayo
    void Update()
    {

        if (isFainted)
        {
            timePassed += Time.deltaTime;
        }
        if (timePassed >= healingTime)
        {
            isFainted = false;
            dogLife = 100;
        }
        
        if (!isFainted)
        {
            if (dogLife <= 0)
            {
                dogLife = 0;
                isFainted = true;
            }

            if (moveInput.x > 0 && !isFacingRight) DogFlip();
            else if (moveInput.x < 0 && isFacingRight) DogFlip();

            if (canBark1)
            {
                Bark1();
            }
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
    void Bark1()
    {
        RaycastHit2D[] hits= Physics2D.RaycastAll(transform.position, transform.right * transform.localScale.x, rayDistance);
        Debug.DrawRay(transform.position, transform.right * transform.localScale.x * rayDistance, Color.yellow);
        dogAnim.SetTrigger("Bark1");

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Debug.Log("Ray hit: " + hit.collider.gameObject.name);
                if (hit.collider.CompareTag("Sheep"))
                {
                    //Cambiar para que las ovejas hagan un MoveTowards ese punto, de esa manera pueden parar si alguien les ataca por ejemplo
                    Rigidbody2D hitRb = hit.collider.GetComponent<Rigidbody2D>();

                    //Si es posible hacer que se desvíen minimamente al correr o que haya posibilidad de ello

                    Vector2 pushDirection = hit.point.x < hitRb.transform.position.x ? transform.right : -transform.right; //Si la pregunta esa es true se hace lo primero, sino lo otro

                    // Aplica la fuerza en la dirección opuesta al lado donde se impactó
                    hitRb.AddForce(pushDirection * rayDistance, ForceMode2D.Impulse);
                    //Activar bool en script oveja respectivo, que se mueva cierta distancia en un move towards(transform.position, transform.position + 5f, ...)
                    
                    hitRb.drag = frenoOvejas; // Establece un valor de drag para que se frene
                }
            }
        }
        canBark1 = false;
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
        /*
         hacer que la primera oveja haga un move towards hasta que esté a x distancia, pero constantly
        cuando entres en el radio de las otras, cada una empieza a seguir a la última con los mismos parámetros

        public Transform leader;  // El objeto líder (el primero en la fila)
        public float distance = 2.0f;  // Distancia entre los seguidores
        public float speed = 3.0f;  // Velocidad de movimiento del seguidor

        private void Update()
        {
            if (leader != null)
            {
                // Calculamos la dirección hacia el líder
                Vector3 direction = leader.position - transform.position;

                // Si estamos demasiado cerca, mantenemos la distancia
                if (direction.magnitude > distance)
                {
                    direction.Normalize();  // Normalizamos para obtener solo la dirección
                    transform.position += direction * speed * Time.deltaTime;  // Movimiento hacia el líder
                }
            }
        }

        array de followers? el primero que toques se almacena en la posición 1, el segundo en la 2, etc., despues cada uno sigue a su numero en el array -1

         */
    }

    #endregion

    public void P2TakesDamage(int damage)
    {
        dogLife -= damage;
        dogAnim.SetTrigger("Hurt");
    }
}
