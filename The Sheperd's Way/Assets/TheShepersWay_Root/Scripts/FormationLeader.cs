using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLeader : MonoBehaviour
{
    [SerializeField] float distanceBetweenSheeps = 2;
    //[SerializeField] bool inLine;
    [SerializeField] Transform[] sheepsInLine;
    int nextSheepIndex;

    private void Start()
    {
        sheepsInLine = new Transform[10];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Sheep"))
        {
            if(!collision.GetComponent<SheepAI>().sheepInLine)
            {
                sheepsInLine[nextSheepIndex] = collision.GetComponent<Transform>();
                nextSheepIndex++;
                Debug.Log("Nueva oveja al array, en el espacio " + (nextSheepIndex - 1));
                collision.GetComponent<SheepAI>().sheepInLine = true;
            }
            else Debug.Log("Esta oveja ya te sigue");
        }
    }
    /*
         hacer que la primera oveja haga un move towards hasta que esté a x distancia, pero constantly
        cuando entres en el radio de las otras, cada una empieza a seguir a la última con los mismos parámetros

        public Transform leader;  // El objeto líder (el primero en la fila)
        public float distance = 2.0f;  // Distancia entre los seguidores
        public float speed = 3.0f;  // Velocidad de movimiento del seguidor

        private void Update()
        {
            if (leader != null)
            {
                // Calculamos la dirección hacia el líder
                Vector3 direction = leader.position - transform.position;

                // Si estamos demasiado cerca, mantenemos la distancia
                if (direction.magnitude > distance)
                {
                    direction.Normalize();  // Normalizamos para obtener solo la dirección
                    transform.position += direction * speed * Time.deltaTime;  // Movimiento hacia el líder
                }
            }
        }

        array de followers? el primero que toques se almacena en la posición 1, el segundo en la 2, etc., despues cada uno sigue a su numero en el array -1

         */
}
