using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepAI : MonoBehaviour
{
    [Header("Sheep Movement Parameters")]
    [SerializeField] float sheepSpeed;
    [SerializeField] bool sheepCanDie;
    public bool dogOrder;

    //Autoreferences
    [SerializeField] BoxCollider2D sheepCol;
    [SerializeField] Rigidbody2D sheepRb;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D sheepCol = GetComponent<BoxCollider2D>();
        sheepCanDie = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!sheepCanDie) sheepCol.enabled = false;

    }

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
}
