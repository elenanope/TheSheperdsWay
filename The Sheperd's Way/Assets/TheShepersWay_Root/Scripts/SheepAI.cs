using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepAI : MonoBehaviour
{
    [SerializeField] float sheepSpeed;
    [SerializeField] bool sheepCanDie;
    [SerializeField] BoxCollider2D sheepCol;

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
}
