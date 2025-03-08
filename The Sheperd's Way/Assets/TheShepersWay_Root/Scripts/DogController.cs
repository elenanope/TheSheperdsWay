using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DogController : MonoBehaviour
{
    [SerializeField] float dogSpeed;
    public bool isFainted;
    [SerializeField] bool canBark1;
    Vector2 moveInput;
    Rigidbody2D dogRb;

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

    void Update()
    {
        if (canBark1)
        {
            //Arreglar todo
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right);
            Debug.DrawRay(transform.position, transform.right * 30f, Color.red);
            canBark1 = false;
            // Verifica si el objeto golpeado tiene el tag especificado
            if (hit.collider.CompareTag("Sheep"))
                {
                    Vector2 direction = (hit.point - (Vector2)transform.position).normalized;
                    // Si el objeto golpeado tiene un Rigidbody2D, mueve el objeto con su Rigidbody2D
                    Rigidbody2D sheepRb = hit.collider.GetComponent<Rigidbody2D>();
                    if (sheepRb != null)
                    {
                        // Mueve el objeto "Sheep" usando su Rigidbody2D
                        sheepRb.velocity = direction * sheepPushDistance;
                    }
                    else
                    {
                        Debug.Log("No hay rigidbody");
                    }

                }
            
        }
    }

    void Move()
    {
        dogRb.velocity = moveInput * dogSpeed;
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
