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
    [SerializeField] float distance;
    [SerializeField] float barkForce;

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
                        hit.collider.GetComponent<SpriteRenderer>().color = Color.yellow;
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
