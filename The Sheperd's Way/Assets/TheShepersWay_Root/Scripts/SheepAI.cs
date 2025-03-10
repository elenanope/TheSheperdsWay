using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepAI : MonoBehaviour
{
    [Header("Sheep Movement Parameters")]
    [SerializeField] int sheepLife = 30;
    [SerializeField] float sheepSpeed;
    [SerializeField] bool sheepCanDie;
    public bool dogOrder;

    //Autoreferences
    BoxCollider2D sheepCol;
    Rigidbody2D sheepRb;
    Animator sheepAnim;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D sheepCol = GetComponent<BoxCollider2D>();
        sheepRb = GetComponent<Rigidbody2D>();
        sheepAnim = GetComponent<Animator>();
        sheepCanDie = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!sheepCanDie) sheepCol.enabled = false;
        if (sheepLife <= 0) SheepDeath();

    }

    #region Sheep Behaviours
    void Chill()
    {
        //En NavMesh
    }

    void Running()
    {

    }

    void FollowingDog()
    {
        //Quizá: si el perro las recoge en fila, que se sigan una a otra
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
        sheepLife -= damage;
    }
    void SheepDeath()
    {
        Debug.Log("A sheep died");
        //sheepAnim.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
