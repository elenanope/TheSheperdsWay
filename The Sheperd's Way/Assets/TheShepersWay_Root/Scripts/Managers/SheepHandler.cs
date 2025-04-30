using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepHandler : MonoBehaviour
{
    [SerializeField] Animator valla1;
    [SerializeField] Animator valla2;
    [SerializeField] bool dogInside;
    [SerializeField] DogController dog;
    public int sheepsInside;
    

    // Update is called once per frame
    void Update()
    {
        if(dog != null)
        {
            if (dog.canBark1 && dogInside)
            {
                if (!valla1.GetBool("openedDoor") && !valla2.GetBool("openedDoor"))
                {
                    valla1.SetBool("openedDoor", true);
                    valla2.SetBool("openedDoor", true);
                }
            }
            if (!dogInside && valla1.GetBool("openedDoor") && valla2.GetBool("openedDoor"))
            {
                valla1.SetBool("openedDoor", false);
                valla2.SetBool("openedDoor", false);
            }
        }
        
        if(sheepsInside >= GameManager.Instance.sheepsAlive *2 && !dogInside)
        {
            valla1.enabled = true;
            valla2.enabled = true;
        }
    }
    private void Start()
    {
        dog = GameObject.Find("P2").GetComponent<DogController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            sheepsInside+=1;
            Debug.Log("oveja dentro");
        }
        if (collision.gameObject.name == "P2")
        {
            dogInside = true;
            Debug.Log("Perro dentro");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            sheepsInside--;
            Debug.Log("oveja fuera");
        }
        if (collision.gameObject.name == "P2")
        {
            dogInside = false;
            Debug.Log("Perro fuera");
        }
    }
}
