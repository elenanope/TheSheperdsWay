using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Librería que permite la carga/descarga de escenas
    
public class SceneHandler : MonoBehaviour
{
    //[SerializeField] AudioSource audioSource;
    public int specificSceneToLoad;
    bool lastScene;
    int sheepsArrived;
    FadingScript fadeo;

    private void Start()
    {
        fadeo = GameObject.Find("Canvas").GetComponent<FadingScript>();
    }
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
            collision.GetComponent<SheepAI>().Running(2);
            //collision.attachedRigidbody.AddForce(transform.right);
            collision.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.name == "P1")
            {
                if(lastScene&&GameManager.Instance.cruelty>=50) fadeo.FadingOut(6);
                else if(lastScene &&GameManager.Instance.cruelty<50) fadeo.FadingOut(3);
                else
                {
                    GameManager.Instance.sheepsAlive = sheepsArrived; //asi solo pasan las vivas a la siguiente pantalla
                                                                      //audioSource.Stop();
                    fadeo.FadingOut(specificSceneToLoad); 
                }
                
            }
            //and el perro esta con las ovejas
            
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

