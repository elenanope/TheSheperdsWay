using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationLeader : MonoBehaviour
{
    public List<Transform> sheepsInLine = new List<Transform>();
    [SerializeField] DogController dogController;

    private void Update()
    {
        if (!dogController.bark2)
        {
            sheepsInLine.Clear();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sheep") && dogController.bark2)
        {
            SheepAI sheepAI = collision.GetComponent<SheepAI>();

            if (!sheepAI.sheepInLine)
            {
                sheepsInLine.Add(collision.transform);  // Agregar oveja a la lista
                sheepAI.sheepInLine = true;

                // Asignar el objeto a seguir
                if (sheepsInLine.Count == 1)
                    sheepAI.objectToFollow = transform;
                else
                    sheepAI.objectToFollow = sheepsInLine[sheepsInLine.Count - 2];

                Debug.Log("Nueva oveja añadida. Total: " + sheepsInLine.Count);
            }
            else Debug.Log("Esta oveja ya está en la línea.");
        }
    }

    public void RemoveSheep(Transform sheep)
    {
        if (sheepsInLine.Contains(sheep))
        {
            sheepsInLine.Remove(sheep);  // Eliminar la oveja de la lista

            // Reasignar los objetivos a seguir
            for (int i = 0; i < sheepsInLine.Count; i++)
            {
                if (i == 0)
                    sheepsInLine[i].GetComponent<SheepAI>().objectToFollow = transform;
                else
                    sheepsInLine[i].GetComponent<SheepAI>().objectToFollow = sheepsInLine[i - 1];
            }

            Debug.Log("Oveja eliminada. Total restante: " + sheepsInLine.Count);
        }
    }
}
