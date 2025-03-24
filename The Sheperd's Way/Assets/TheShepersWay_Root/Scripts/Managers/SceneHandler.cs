using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Librería que permite la carga/descarga de escenas
    
public class SceneHandler : MonoBehaviour
{
    //[SerializeField] AudioSource audioSource;
    public int specificSceneToLoad; 
    int sceneToLoad;
    int sheepsArrived;

    private void Update()
    {
        /* poner esto de manera que no pase constantly o no ponerlo así
        if(GameManager.Instance.currentGameState == 0)
        {
            SceneManager.LoadScene(3);
        }
        */
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            sheepsArrived++;
            collision.attachedRigidbody.AddForce(transform.right);
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.sheepsAlive = sheepsArrived; //asi solo pasan las vivas a la siguiente pantalla
            //audioSource.Stop();
            SceneManager.LoadScene(specificSceneToLoad);
        }
    }
    public void SceneLoader(int sceneToLoad)
    {
        //audioSource.Stop();
        SceneManager.LoadScene(sceneToLoad);
    }
    public void SceneLoaderOnPlay()
    {
        //audioSource.Stop();
        SceneManager.LoadScene(specificSceneToLoad);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

}

