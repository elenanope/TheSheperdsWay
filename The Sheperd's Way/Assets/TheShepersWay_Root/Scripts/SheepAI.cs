using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    //Autoreferences
    BoxCollider2D sheepCol;
    Rigidbody2D sheepRb;
    Animator sheepAnim;

    private void OnDisable()
    {
        sheepInLine = false;
        objectToFollow = null;
        leader.RemoveSheep(transform);
    }
    void Start()
    {
        leader = FindObjectOfType<FormationLeader>();
        sheepCol = GetComponent<BoxCollider2D>();
        sheepRb = GetComponent<Rigidbody2D>();
        sheepAnim = GetComponent<Animator>();
        sheepCanDie = true;
    }

    void Update()
    {
        if (!sheepCanDie) sheepCol.enabled = false;
        if (sheepLife <= 0) SheepDeath();
        if (sheepInLine) FollowingDog();
    }

    #region Sheep Behaviours
    void Chill()
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
                transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, sheepSpeed * Time.deltaTime);
            }
            else transform.position = transform.position;
        }
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
