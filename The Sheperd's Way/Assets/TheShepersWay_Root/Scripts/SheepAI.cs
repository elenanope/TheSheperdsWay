using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class SheepAI : MonoBehaviour
{
    [Header("Sheep Movement Parameters")]
    [SerializeField] int sheepLife = 30;
    [SerializeField] float sheepSpeed;
    [SerializeField] bool sheepCanDie;
    public bool sheepInLine;
    public Transform objectToFollow;
    [SerializeField] float distanceBetween = 2f;
    FormationLeader leader;
    int direction = 1; // 1 = derecha, -1 = izquierda

    [SerializeField] float wanderingSpeed = 5;
    [SerializeField] bool isWandering;
    [SerializeField] bool isWalking;
    [SerializeField] bool isFacingRight = true;

    //Autoreferences
    BoxCollider2D sheepCol;
    Rigidbody2D sheepRb;
    Animator sheepAnim;

    private void OnDisable()
    {
        StopFollowing();
        StopAllCoroutines();
        isWandering = false;
        isWalking = false;
    }
    void Start()
    {
        leader = FindObjectOfType<FormationLeader>();
        sheepCol = GetComponent<BoxCollider2D>();
        sheepRb = GetComponent<Rigidbody2D>();
        sheepAnim = GetComponent<Animator>();
        sheepCanDie = true;
    }
    private void FixedUpdate()
    {
        if (!sheepInLine)
        {
            if (!isWandering) StartCoroutine(Wander());

            if (isWalking)
            {
                sheepRb.velocity = new Vector2(direction * wanderingSpeed, sheepRb.velocity.y);
            }
            else
            {
                sheepRb.velocity = new Vector2(0, sheepRb.velocity.y);
            }
        }
        else
        {
            StopAllCoroutines();
            isWandering = false;
            isWalking = false;
        }
    }

    void Update()
    {
        if (!sheepCanDie) sheepCol.enabled = false;
        if (sheepLife <= 0) SheepDeath();
        if (sheepInLine) FollowingDog();
        if (leader.sheepsInLine.Count == 0) StopFollowing();

        
        if (isWalking) sheepAnim.SetBool("Walk", true);
        else sheepAnim.SetBool("Walk", false);

        //flipearla segun pa donde vaya
            
    }

    #region Sheep Behaviours
    void Wandering()
    {
        //En NavMesh
    }

    void Running()
    {

    }

    void FollowingDog ()
    {
        if (objectToFollow != null)
        {
            float currentDistance = Vector2.Distance(transform.position, objectToFollow.position);

            if (currentDistance >= distanceBetween)
            {
                isWalking = true;
                transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, sheepSpeed * Time.deltaTime);
                if (transform.position.x > objectToFollow.position.x && isFacingRight) Flip();
                else if (transform.position.x < objectToFollow.position.x && !isFacingRight) Flip();
            }
            else 
            {
                transform.position = transform.position;
                isWalking = false;
            }
        }
    }

    void StopFollowing()
    {
        sheepInLine = false;
        objectToFollow = null;
        leader.RemoveSheep(transform);
    }

    void FleeingFromEnemy()
    {

    }

    void FleeingFromBattle() //opcional
    {

    }

    void HeldByShepherd()
    {

    }
    #endregion


    IEnumerator Wander()
    {
        int walkWait = Random.Range(4, 11);
        int walkTime = Random.Range(0, 3);
        int flipWait = Random.Range(0, 8);
        bool flippingASheep = Random.Range(0, 2) == 0;

        isWandering = true;

        yield return new WaitForSeconds(walkWait);

        isWalking = true;

        yield return new WaitForSeconds(walkTime);

        isWalking = false;

        yield return new WaitForSeconds(flipWait);

        if (flippingASheep) Flip();

        isWandering = false;

        //Meter tmb que pueda bajar la cabeza tipo ñam ñam hierba

    }

    void Flip()
    {
        direction *= -1;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
    public void SheepTakesDamage(int damage)
    {
        sheepAnim.SetTrigger("Hurt");
        sheepLife -= damage;
    }
    void SheepDeath()
    {
        Debug.Log("A sheep died");
        sheepAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    } 


}
