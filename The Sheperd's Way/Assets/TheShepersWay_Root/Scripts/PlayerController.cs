using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] bool isFacingRight;
    Vector2 moveInput;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (moveInput.x > 0 && !isFacingRight) Flip();
        else if (moveInput.x < 0 && isFacingRight) Flip();
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


    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
            moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {

    }

    #endregion
}
