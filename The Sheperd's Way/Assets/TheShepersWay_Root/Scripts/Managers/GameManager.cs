using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) Debug.Log("GameManager is missing!");
            return instance;
        }
    }
    public int totalLife = 100;
    public int sheepsAlive = 10;
    public enum GameState { gameOver, gameStarted, gamePaused, gameCompleted }
    public GameState currentGameState = GameState.gameStarted;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantiene el objeto al cambiar de escena
        }
        else Destroy(gameObject); // Si ya hay un GameManager, destruye el nuevo
        
    }

    private void Update()
    {
        //Condición para perder la partida
        if (totalLife <= 0)
        {
            totalLife = 0;
            currentGameState = GameState.gameOver;
            Debug.Log("No tienes más vida!");
        }
        if (sheepsAlive <= 0)
        {
            sheepsAlive = 0;
            currentGameState = GameState.gameOver;
            Debug.Log("Se han muerto todas las ovejas!");
        }

    }

}
