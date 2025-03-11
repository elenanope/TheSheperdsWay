using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SheepAI : MonoBehaviour
{
    [Header("Sheep Movement Parameters")]
    [SerializeField] int sheepLife = 30;
    [SerializeField] float sheepSpeed;
    [SerializeField] bool sheepCanDie;
    public bool sheepInLine;
    public Transform objectToFollow;
    [SerializeField] float distanceBetween =2f;
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
        objectToFollow = null;
    }

    // Update is called once per frame
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

            // Si estamos demasiado cerca, mantenemos la distancia
            if (currentDistance >= distanceBetween)
            {
                transform.position = Vector3.MoveTowards(transform.position, objectToFollow.position, sheepSpeed * Time.deltaTime);
            }
            else transform.position = transform.position;
            
        }
        

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
